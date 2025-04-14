namespace CrowdedBackend.Services.CalculatePositions;

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class CircleUtils
{
    private const double RSSI_TO_LENGTH = 2.0; 
    private const double CIRCLE_EXTRA_SIZE = 2.0;
    private const double SMALL = 1e-10;

    private readonly RaspOutputData? _rasp1OutputData;
    private readonly RaspOutputData? _rasp2OutputData;
    private readonly RaspOutputData? _rasp3OutputData;

    private readonly Point _rasp1Point;
    private readonly Point _rasp2Point;
    private readonly Point _rasp3Point;

    public CircleUtils(String rasp1Input, Point rasp1Point, String rasp2Input, Point rasp2Point, String rasp3Input, Point rasp3Point)
    {
        this._rasp1OutputData = JsonConvert.DeserializeObject<RaspOutputData>(rasp1Input);
        this._rasp2OutputData = JsonConvert.DeserializeObject<RaspOutputData>(rasp2Input);
        this._rasp3OutputData = JsonConvert.DeserializeObject<RaspOutputData>(rasp3Input);
        
        this._rasp1Point = rasp1Point;
        this._rasp2Point = rasp2Point;
        this._rasp3Point = rasp3Point;
    }

    public Point? CalculatePosition()
    {
        try
        {
            if (_rasp1OutputData is null || _rasp2OutputData is null || _rasp3OutputData is null)
            {
                throw new Exception("One or more raspOutput values are invalid");
            }

            if (_rasp1Point is null || _rasp2Point is null || _rasp3Point is null)
            {
                throw new Exception("One or more raspPoint values are invalid");
            }
            
            foreach (var rasp1Data in _rasp1OutputData.events)
            {
                var macAddress = rasp1Data.macAddress;
                var rasp2Match = _rasp2OutputData.events.First(rasp2Data => rasp2Data.macAddress == macAddress);
                var rasp3Match = _rasp3OutputData.events.First(rasp3Data => rasp3Data.macAddress == macAddress);

                List<Circle> circles = new List<Circle>();
                circles.Add(new Circle(this._rasp1Point.X, this._rasp1Point.Y, this.RSSIToLength(rasp1Data.signalStrengthRSSI)));
                circles.Add(new Circle(this._rasp2Point.X, this._rasp2Point.Y, this.RSSIToLength(rasp2Match.signalStrengthRSSI)));
                circles.Add(new Circle(this._rasp3Point.X, this._rasp3Point.Y, this.RSSIToLength(rasp3Match.signalStrengthRSSI)));

                return EstimatedPoint(circles);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
        
        return null;
    }

    private double RSSIToLength(double rssi)
    {
        // TODO:Actually calculate
        return rssi + RSSI_TO_LENGTH + CIRCLE_EXTRA_SIZE;
    }

    private Point EstimatedPoint(List<Circle> circles)
    {
        //TODO: ALTERNATIVELY, use triangle points to determine ish-middle if this is too slow
        var intersectionPoints = GetIntersectionPoints(circles);
        var innerPoints = intersectionPoints.Where(p => ContainedInCircles(p, circles)).ToList();
        if (innerPoints is null || innerPoints.Count == 0)
        {
            throw new Exception("No inner points in area");
        }
        
        return GetCenter(innerPoints);
    }

    public bool ContainedInCircles(Point point, List<Circle> circles)
    {
        return circles.All(c => Distance(point, new Point(c.X, c.Y)) <= c.Radius + SMALL);
    }

    public List<Point> GetIntersectionPoints(List<Circle> circles)
    {
        var result = new List<Point>();
        for (int i = 0; i < circles.Count; i++)
        {
            for (int j = i + 1; j < circles.Count; j++)
            {
                var intersect = CircleCircleIntersection(circles[i], circles[j]);
                foreach (var p in intersect)
                {
                    p.ParentIndex.AddRange(new[] { i, j });
                    result.Add(p);
                }
            }
        }
        return result;
    }

    public double Distance(Point p1, Point p2)
    {
        var dx = p1.X - p2.X;
        var dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public double Distance(Circle c1, Circle c2)
    {
        return Distance(new Point(c1.X, c1.Y), new Point(c2.X, c2.Y));
    }

    public  List<Point> CircleCircleIntersection(Circle c1, Circle c2)
    {
        var d = Distance(c1, c2);
        var r1 = c1.Radius;
        var r2 = c2.Radius;

        if (d >= r1 + r2 || d <= Math.Abs(r1 - r2))
            return new List<Point>();

        var a = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
        var h = Math.Sqrt(r1 * r1 - a * a);
        var x0 = c1.X + a * (c2.X - c1.X) / d;
        var y0 = c1.Y + a * (c2.Y - c1.Y) / d;
        var rx = -(c2.Y - c1.Y) * (h / d);
        var ry = -(c2.X - c1.X) * (h / d);

        return new List<Point>
        {
            new(x0 + rx, y0 - ry),
            new(x0 - rx, y0 + ry)
        };
    }

    public Point GetCenter(List<Point> points)
    {
        var center = new Point(0, 0);
        foreach (var p in points)
        {
            center.X += p.X;
            center.Y += p.Y;
        }

        center.X /= points.Count;
        center.Y /= points.Count;
        return center;
    }
}

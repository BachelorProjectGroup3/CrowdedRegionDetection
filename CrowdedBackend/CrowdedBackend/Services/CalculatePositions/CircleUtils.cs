using CrowdedBackend.Models;

namespace CrowdedBackend.Services.CalculatePositions;

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class CircleUtils
{
    private const double RSSI_TO_LENGTH = 3;
    private const double CIRCLE_EXTRA_SIZE = 20;
    private const double SMALL = 1e-10;

    private List<RaspOutputData?> raspOutputData = new List<RaspOutputData?>();
    private List<Point> raspPoints = new List<Point>();

    public int AddData(RaspOutputData raspInput, Point raspPoint)
    {
        if (raspOutputData.Count < 3)
        {
            this.raspOutputData.Add(raspInput);
            this.raspPoints.Add(raspPoint);
        }

        return raspOutputData.Count;
    }

    public void WipeData()
    {
        this.raspOutputData = new List<RaspOutputData?>();
        this.raspPoints = new List<Point>();
    }

    public List<Point> CalculatePosition()
    {
        List<Point> points = new List<Point>();
        try
        {
            if (raspOutputData is null || raspOutputData.Count != 3)
            {
                throw new Exception("raspOutput data values are invalid");
            }

            if (raspPoints is null || raspPoints.Count != 3)
            {
                throw new Exception("raspPoints values are invalid");
            }
            foreach (var rasp1Data in raspOutputData[0].Events)
            {
                var macAddress = rasp1Data.MacAddress;
                var rasp2Match = raspOutputData[1].Events.First(rasp2Data => rasp2Data.MacAddress == macAddress);
                var rasp3Match = raspOutputData[2].Events.First(rasp3Data => rasp3Data.MacAddress == macAddress);

                List<Circle> circles = new List<Circle>();
                circles.Add(new Circle(this.raspPoints[0].X, this.raspPoints[0].Y, this.RSSIToLength(rasp1Data.Rssi)));
                circles.Add(new Circle(this.raspPoints[1].X, this.raspPoints[1].Y, this.RSSIToLength(rasp2Match.Rssi)));
                circles.Add(new Circle(this.raspPoints[2].X, this.raspPoints[2].Y, this.RSSIToLength(rasp3Match.Rssi)));

                points.Add(EstimatedPoint(circles));
            }

            return points;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return null;
    }

    private double RSSIToLength(double rssi)
    {
        // TODO:Actually calculate
        return -rssi * RSSI_TO_LENGTH + CIRCLE_EXTRA_SIZE;
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

    public List<Point> CircleCircleIntersection(Circle c1, Circle c2)
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

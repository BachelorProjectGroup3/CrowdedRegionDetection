using NuGet.Protocol;

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

    private List<RaspOutputData>? rasp1OutputData;
    private List<RaspOutputData>? rasp2OutputData;
    private List<RaspOutputData>? rasp3OutputData;

    private Point rasp1Point;
    private Point rasp2Point;
    private Point rasp3Point;

    public CircleUtils(String rasp1Input, String rasp2Input, String rasp3Input)
    {
        this.rasp1OutputData = JsonConvert.DeserializeObject<List<RaspOutputData>>(rasp1Input);
        this.rasp2OutputData = JsonConvert.DeserializeObject<List<RaspOutputData>>(rasp2Input);
        this.rasp3OutputData = JsonConvert.DeserializeObject<List<RaspOutputData>>(rasp3Input);
        
        //TODO:GET rasp123Point positions from data
    }

    // public Point CalculatePosition()
    public double CalculatePosition()
    {
        try
        {
            if (rasp1OutputData is null || rasp2OutputData is null || rasp3OutputData is null)
            {
                throw new Exception("One or more inputs are null");
            }
            
            foreach (var rasp1Data in rasp1OutputData)
            {
                var macAddress = rasp1Data.macAddress;
                var rasp2Match = rasp2OutputData.First(rasp2Data => rasp2Data.macAddress == macAddress);
                var rasp3Match = rasp3OutputData.First(rasp3Data => rasp3Data.macAddress == macAddress);

                List<Circle> circles = new List<Circle>();
                circles.Add(new Circle(this.rasp1Point.X, this.rasp1Point.Y, this.RSSIToLength(rasp1Data.signalStrengthRSSI)));
                circles.Add(new Circle(this.rasp2Point.X, this.rasp2Point.Y, this.RSSIToLength(rasp2Match.signalStrengthRSSI)));
                circles.Add(new Circle(this.rasp3Point.X, this.rasp3Point.Y, this.RSSIToLength(rasp3Match.signalStrengthRSSI)));

                return IntersectionArea(circles);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
        
        return 0.0;
    }

    private double RSSIToLength(double rssi)
    {
        return rssi + RSSI_TO_LENGTH + CIRCLE_EXTRA_SIZE;
    }

    private double IntersectionArea(List<Circle> circles, IntersectionStats stats = null)
    {
        var intersectionPoints = GetIntersectionPoints(circles);
        var innerPoints = intersectionPoints.Where(p => ContainedInCircles(p, circles)).ToList();

        double arcArea = 0, polygonArea = 0;
        var arcs = new List<Arc>();

        if (innerPoints.Count > 1)
        {
            var center = GetCenter(innerPoints);
            foreach (var p in innerPoints)
            {
                p.Angle = Math.Atan2(p.X - center.X, p.Y - center.Y);
            }

            innerPoints.Sort((a, b) => b.Angle.CompareTo(a.Angle));

            var p2 = innerPoints.Last();
            foreach (var p1 in innerPoints)
            {
                polygonArea += (p2.X + p1.X) * (p1.Y - p2.Y);

                var midPoint = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                Arc arc = null;

                foreach (var idx in p1.ParentIndex)
                {
                    if (p2.ParentIndex.Contains(idx))
                    {
                        var circle = circles[idx];
                        var a1 = Math.Atan2(p1.X - circle.X, p1.Y - circle.Y);
                        var a2 = Math.Atan2(p2.X - circle.X, p2.Y - circle.Y);

                        var angleDiff = a2 - a1;
                        if (angleDiff < 0)
                            angleDiff += 2 * Math.PI;

                        var a = a2 - angleDiff / 2;
                        var arcX = circle.X + circle.Radius * Math.Sin(a);
                        var arcY = circle.Y + circle.Radius * Math.Cos(a);
                        var width = Distance(midPoint, new Point(arcX, arcY));

                        if (width > circle.Radius * 2)
                            width = circle.Radius * 2;

                        if (arc == null || arc.Width > width)
                        {
                            arc = new Arc
                            {
                                Circle = circle,
                                Width = width,
                                P1 = p1,
                                P2 = p2
                            };
                        }
                    }
                }

                if (arc != null)
                {
                    arcs.Add(arc);
                    arcArea += CircleArea(arc.Circle.Radius, arc.Width);
                    p2 = p1;
                }
            }
        }
        else
        {
            var smallest = circles.OrderBy(c => c.Radius).First();
            var disjoint = circles.Any(c => Distance(c, smallest) > Math.Abs(smallest.Radius - c.Radius));

            if (disjoint)
            {
                arcArea = polygonArea = 0;
            }
            else
            {
                arcArea = smallest.Radius * smallest.Radius * Math.PI;
                arcs.Add(new Arc
                {
                    Circle = smallest,
                    Width = smallest.Radius * 2,
                    P1 = new Point(smallest.X, smallest.Y + smallest.Radius),
                    P2 = new Point(smallest.X - SMALL, smallest.Y + smallest.Radius)
                });
            }
        }

        polygonArea /= 2;

        if (stats != null)
        {
            stats.Area = arcArea + polygonArea;
            stats.ArcArea = arcArea;
            stats.PolygonArea = polygonArea;
            stats.Arcs = arcs;
            stats.InnerPoints = innerPoints;
            stats.IntersectionPoints = intersectionPoints;
        }

        return arcArea + polygonArea;
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

    public double CircleArea(double r, double width)
    {
        return r * r * Math.Acos(1 - width / r) - (r - width) * Math.Sqrt(width * (2 * r - width));
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

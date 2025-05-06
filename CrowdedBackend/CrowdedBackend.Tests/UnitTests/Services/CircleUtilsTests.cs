using Xunit;
using CrowdedBackend.Services.CalculatePositions;
using CrowdedBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CrowdedBackend.Tests.UnitTests.Services
{
    public class CircleUtilsTests
    {
        /*
         * Test that ensure three RSSI readings and their geometric logic produce a usable position
         */
        [Fact]
        public void CalculatePosition_WithValidData_ReturnsPosition()
        {
            // Arrange
            var circleUtils = new CircleUtils();

            var point1 = new Point(0, 0);
            var point2 = new Point(10, 0);
            var point3 = new Point(5, 10);

            var mac = "AA:BB:CC:DD";

            var data1 = new RaspOutputData
            {
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680}}
            };

            var data2 = new RaspOutputData
            {
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680}}
            };

            var data3 = new RaspOutputData
            {
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680}}
            };

            circleUtils.AddData(data1, point1);
            circleUtils.AddData(data2, point2);
            circleUtils.AddData(data3, point3);

            // Act
            var result = circleUtils.CalculatePosition();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.IsType<Point>(result[0]);

            // Optionally print or validate X/Y ranges
            Assert.InRange(result[0].X, 3.0, 7.0);
            Assert.InRange(result[0].Y, 3.0, 7.0);
        }
        
        /*
         * Test if 3 overlapping circles has points that exists in 2 circles
         */
        [Fact]
        public void GetIntersectionPoints_WithThreeIntersectingCircles_ReturnsCorrectPoints()
        {
            // Arrange
            var circleUtils = new CircleUtils();
            var circles = new List<Circle>
            {
                new (0, 0, 5),
                new (4, 0, 5),
                new (2, 4, 5)
            };

            // Act
            var result = circleUtils.GetIntersectionPoints(circles);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0); // Should have some intersection points
            Assert.All(result, p => Assert.True(p.ParentIndex.Count == 2)); // Ensure each point has parent indices
        }
        
        /*
         * Test if CricleCirle intersection has two intersections points
         */
        [Fact]
        public void CircleCircleIntersection_IntersectingCircles_ReturnsTwoPoints()
        {
            // Arrange
            var circle1 = new Circle(0, 0, 5);
            var circle2 = new Circle(6, 0, 5);
            var utils = new CircleUtils();

            // Act
            var result = utils.CircleCircleIntersection(circle1, circle2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, point =>
            {
                // Check that points are roughly between the two centers
                Assert.InRange(point.X, 0, 6);
                Assert.InRange(point.Y, -5, 5);
            });
        }
        
        /*
         * Test if CircleCircle intersection has no intersection circles.
         * Should return empty list
         */
        [Fact]
        public void CircleCircleIntersection_NonIntersectingCircles_ReturnsEmptyList()
        {
            var circle1 = new Circle(0, 0, 2);
            var circle2 = new Circle(10, 0, 2);
            var utils = new CircleUtils();

            var result = utils.CircleCircleIntersection(circle1, circle2);

            Assert.Empty(result);
        }
    }
}
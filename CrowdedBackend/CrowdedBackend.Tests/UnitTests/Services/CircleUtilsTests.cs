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
        /// <summary>
        ///     testing to calculatePosition by creating correct points
        ///     Using the method in CircleUtils and verify its in range
        /// </summary>
        /// <remark>
        ///      Using the method in CircleUtils and verify its in range
        /// </remark>
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
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680 } }
            };

            var data2 = new RaspOutputData
            {
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680 } }
            };

            var data3 = new RaspOutputData
            {
                Events = new List<RaspEvent> { new RaspEvent { MacAddress = mac, Rssi = -50, UnixTimestamp = 1746088680 } }
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
            Assert.InRange(result[0].X, 3.0, 10.0);
            Assert.InRange(result[0].Y, 3.0, 10.0);
        }

        /// <summary>
        ///     Correctly finds all pairwise intersection points between three overlapping circles
        /// </summary>
        /// <remark>
        ///     Confirms the result isn't null
        ///     Confirms that some intersection points were found.
        ///     Ensures every point knows which two circles created it (ParentIndex has 2 items).
        /// </remark>
        [Fact]
        public void GetIntersectionPoints_WithThreeIntersectingCircles_ReturnsCorrectPoints()
        {
            var circleUtils = new CircleUtils();
            var circles = new List<Circle>
            {
                new (0, 0, 5),
                new (4, 0, 5),
                new (2, 4, 5)
            };

            var result = circleUtils.GetIntersectionPoints(circles);

            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.All(result, p => Assert.True(p.ParentIndex.Count == 2));
        }


        /// <summary>
        ///     Each intersection point’s X and Y coordinates are within reasonable bounds
        ///     — roughly between the centers of the two circles.
        /// </summary>
        /// <remark>
        ///     Two overlapping circles produce exactly two intersection points.
        ///     The intersection points fall within expected coordinate ranges.
        /// </remark>
        [Fact]
        public void CircleCircleIntersection_IntersectingCircles_ReturnsTwoPoints()
        {
            var circle1 = new Circle(0, 0, 5);
            var circle2 = new Circle(6, 0, 5);
            var utils = new CircleUtils();

            var result = utils.CircleCircleIntersection(circle1, circle2);

            Assert.Equal(2, result.Count);
            Assert.All(result, point =>
            {
                // Check that points are roughly between the two centers
                Assert.InRange(point.X, 0, 6);
                Assert.InRange(point.Y, -5, 5);
            });
        }


        /// <summary>
        ///     testing if there is no intersecting circles
        /// </summary>
        /// <remark>
        ///     Should pass because the circles has no intersection
        /// </remark>
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
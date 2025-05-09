using Xunit;
using CrowdedBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using CrowdedBackend.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace CrowdedBackend.Tests.UnitTests.Controllers
{
    public class DetectedDevicesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly MyDbContext _context;
        private readonly DetectedDevicesController _controller;

        public DetectedDevicesControllerTests(CustomWebApplicationFactory factory)
        {
            // Use the factory to create a scope for the DB context
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new DetectedDevicesController(_context, null);
        }

        [Fact]
        public async Task PostDetectedDevices_ReturnsOkResult()
        {
            // Arrange
            var detectedDevice = new DetectedDevice
            {
                DetectedDeviceId = 1,
                VenueID = 1,
                DeviceX = 3,
                DeviceY = 4,
                Timestamp = 1745562072611
            };

            // Act
            var result = await _controller.PostDetectedDevice(detectedDevice);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDetectedDevice = Assert.IsType<DetectedDevice>(createdAtActionResult.Value);
            Assert.Equal(detectedDevice.DetectedDeviceId, returnedDetectedDevice.DetectedDeviceId);

        }
    }
}
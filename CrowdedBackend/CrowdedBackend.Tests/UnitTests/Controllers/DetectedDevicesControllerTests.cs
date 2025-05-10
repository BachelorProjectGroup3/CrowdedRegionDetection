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
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new DetectedDevicesController(_context, null);
        }

        /// <summary>
        ///     Creating a detectedDevice 
        /// </summary>
        /// <remark>
        ///     Expected to pass by compairng DeviceId and expected DeviceId
        ///     Also checking its type
        /// </remark>
        [Fact]
        public async Task PostDetectedDevices_ReturnsOkResult()
        {
            var detectedDevice = new DetectedDevice
            {
                DetectedDeviceId = 1,
                VenueID = 1,
                DeviceX = 3,
                DeviceY = 4,
                Timestamp = 1745562072611
            };

            var result = await _controller.PostDetectedDevice(detectedDevice);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDetectedDevice = Assert.IsType<DetectedDevice>(createdAtActionResult.Value);
            Assert.Equal(detectedDevice.DetectedDeviceId, returnedDetectedDevice.DetectedDeviceId);

        }
    }
}
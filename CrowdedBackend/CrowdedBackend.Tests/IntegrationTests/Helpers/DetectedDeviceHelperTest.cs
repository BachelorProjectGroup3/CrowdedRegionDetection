
using System.Text.Json;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using CrowdedBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace CrowdedBackend.Tests.IntegrationTests.Helpers
{
    public class DetectedDeviceHelperTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly MyDbContext _context;
        private readonly CircleUtils _circleUtils;
        private readonly DetectedDeviceHelper _helper;


        // Constructor to inject the dependencies
        public DetectedDeviceHelperTest(CustomWebApplicationFactory factory)
        {
            // Use the factory to create a scope for the DB context
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _circleUtils = scope.ServiceProvider.GetRequiredService<CircleUtils>();  // Ensure CircleUtils is injected
            _helper = new DetectedDeviceHelper(_context, _circleUtils, null);  // Ensure DetectedDeviceHelper is injected
        }

        [Fact]
        public async Task HandleRaspPostRequest_test()
        {

            // Arrange
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Read the raspOutputData from the JSON file
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "raspOutputData.json");

            var raspOutputData = JsonSerializer.Deserialize<RaspOutputData>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(raspOutputData);
            var result = await _helper.HandleRaspPostRequest(raspOutputData, now);

            Assert.NotNull(result);
        }
    }
}
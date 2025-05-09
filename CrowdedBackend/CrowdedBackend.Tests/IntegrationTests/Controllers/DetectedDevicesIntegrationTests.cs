using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CrowdedBackend.Models;
using Xunit;
using Xunit.Abstractions;


namespace CrowdedBackend.Tests.IntegrationTests.Controllers
{
    [Collection("Non-Parallel Collection")]
    public class DetectedDevicesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly ITestOutputHelper _TestOutput;
        private long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


        public DetectedDevicesIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper TestOutput)
        {
            _TestOutput = TestOutput;
            _factory = factory;
            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task GetHeatmapAtSpecificTime_returnBitstring()
        {
            // Arrange: Create a Venue object to send to the API
            var venue = new Venue { VenueName = "TestVenue" };

            // Act: Send the POST request to the /api/Venue endpoint
            await _client.PostAsJsonAsync("/api/Venue", venue);


            // Arrange
            timestamp -= (timestamp % 30000); // same TimeInterval used in controller
            var detectedDevice = new DetectedDevice
            {
                DeviceX = 50,
                DeviceY = 100,
                Timestamp = timestamp,
                VenueID = venue.VenueID,
            };

            await _client.PostAsJsonAsync("/api/DetectedDevices", detectedDevice);

            var result = await _client.GetAsync($"/api/DetectedDevices/getHeatmapAtSpecificTime/{timestamp}");
            var responseContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(responseContent));
            _TestOutput.WriteLine(responseContent);
        }

        [Fact]
        public async Task HandleRaspPostRequest_test()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "raspOutputData.json");

            var raspOutputData = JsonSerializer.Deserialize<RaspOutputData>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.PostAsJsonAsync("/api/detectedDevices/uploadMultiple", raspOutputData);
            _TestOutput.WriteLine($"response : {response}");


            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CanGetDetectedDevices()
        {
            timestamp -= (timestamp % 30000); // same TimeInterval used in controller
            var detectedDevice = new DetectedDevice
            {
                DeviceX = 50,
                DeviceY = 70,
                Timestamp = timestamp,
                VenueID = 1
            };

            await _client.PostAsJsonAsync("/api/DetectedDevices", detectedDevice);

            var response = await _client.GetAsync("/api/DetectedDevices");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var devices = JsonSerializer.Deserialize<List<DetectedDevice>>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(devices);
            Assert.NotEmpty(devices);

            _TestOutput.WriteLine($"Found {devices.Count} detected devices.");
        }

    }
}

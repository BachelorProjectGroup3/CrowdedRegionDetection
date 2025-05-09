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
        


        public DetectedDevicesIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper TestOutput)
        {
            _TestOutput = TestOutput;
            _factory = factory;
            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task GetLatestValidHeatmap_returnBitstring()
        {
            var result = await _client.GetAsync($"api/DetectedDevices/getLatestValidHeatmap");
            var responseContent = await result.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();
            
            Assert.False(string.IsNullOrWhiteSpace(responseContent));
        }

        [Fact]
        public async Task HandleRaspPostRequest_test()
        {
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "raspOutputData.json");

            var raspOutputData = JsonSerializer.Deserialize<RaspOutputData>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var response = await _client.PostAsJsonAsync("/api/detectedDevices/uploadMultiple", raspOutputData);
            response.EnsureSuccessStatusCode();
            _TestOutput.WriteLine($"response : {response}");
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CanGetDetectedDevices()
        {
            
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

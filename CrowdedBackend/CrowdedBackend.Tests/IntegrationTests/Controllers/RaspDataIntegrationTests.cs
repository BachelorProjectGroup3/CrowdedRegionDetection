using System.Net;
using System.Net.Http.Json;
using CrowdedBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CrowdedBackend.Tests.IntegrationTests.Controllers
{
    public class RaspDataIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public RaspDataIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

        }

        /// <summary>
        ///     testing post endpoint for RaspData
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the response code
        ///     Also if the returned MacAddress is the same as expected
        /// </remark>
        [Fact]
        public async Task PostRaspData_SavesRaspData_ReturnsCreated()
        {
            var RaspData = new RaspData { MacAddress = "24:58:46:97:75:3F", RaspId = 3, Rssi = -82, UnixTimestamp = 1746530400000 };

            var response = await _client.PostAsJsonAsync("/api/RaspData", RaspData);
            response.EnsureSuccessStatusCode();

            var returned = await response.Content.ReadFromJsonAsync<RaspData>();

            Assert.Equal("24:58:46:97:75:3F", returned.MacAddress);
        }
    }
}

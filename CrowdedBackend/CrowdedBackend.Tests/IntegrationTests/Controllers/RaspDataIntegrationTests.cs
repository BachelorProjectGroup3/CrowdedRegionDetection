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

        [Fact]
        public async Task PostRaspData_SavesRaspData_ReturnsCreated()
        {
            // Arrange: Create a Venue object to send to the API
            var RaspData = new RaspData { MacAddress = "24:58:46:97:75:3F", RaspId = 3, Rssi = -82, UnixTimestamp = 1746530400000 };

            // Act: Send the POST request to the /api/Venue endpoint
            var response = await _client.PostAsJsonAsync("/api/RaspData", RaspData);

            // Assert: Ensure the response status code is 201 (Created)
            response.EnsureSuccessStatusCode();

            // Deserialize the returned content to check the saved venue
            var returned = await response.Content.ReadFromJsonAsync<RaspData>();

            // Assert: Check if the returned venue is as expected
            Assert.Equal("24:58:46:97:75:3F", returned.MacAddress);
        }

        /*
        [Fact]
        public async Task GetVenue__ReturnsVenue()
        {
            // Arrange
            const int id = 4;
            var response = await _client.GetAsync($"/api/Venue/{id}");
            response.EnsureSuccessStatusCode();
            var returned = await response.Content.ReadFromJsonAsync<Venue>();
            
            // Assert
            Assert.Equal(id, returned.VenueID);
            Assert.Equal("TestVenue", returned.VenueName);
        }

        [Fact]
        public async Task GetVenue_UpdateVenue_ReturnsUpdatedVenue()
        {
            // Arrange
            const int id = 4;
            var getResponse = await _client.GetAsync($"/api/Venue/{id}");
            getResponse.EnsureSuccessStatusCode();
            var originalVenue = await getResponse.Content.ReadFromJsonAsync<Venue>();

            Assert.NotNull(originalVenue);

            // Act - Modify the venue
            originalVenue.VenueName = "Updated Venue Name";
    
            var putResponse = await _client.PutAsJsonAsync($"/api/Venue/{id}", originalVenue);
            putResponse.EnsureSuccessStatusCode();

            // Assert - Get again and verify the updated name
            var confirmResponse = await _client.GetAsync($"/api/Venue/{id}");
            confirmResponse.EnsureSuccessStatusCode();
            var updatedVenue = await confirmResponse.Content.ReadFromJsonAsync<Venue>();

            Assert.Equal("Updated Venue Name", updatedVenue.VenueName);
        }

        [Fact]
        public async Task DeleteVenue_ReturnsDeletedVenue_And_CannotBeFoundAfter()
        {
            // Arrange
            var venue = new Venue { VenueID = 99, VenueName = "Venue to Delete" };
            var postResponse = await _client.PostAsJsonAsync("/api/Venue", venue);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/Venue/{venue.VenueID}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getAfterDelete = await _client.GetAsync($"/api/Venue/{venue.VenueID}");
            Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }
        */

    }
}

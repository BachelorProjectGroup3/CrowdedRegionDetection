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
    public class VenueIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public VenueIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        /// <summary>
        ///     Testing our endpoint for posting a Venue
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the response code
        ///     Also if the returned name is the same as expected
        /// </remark>
        [Fact]
        public async Task PostVenue_SavesVenue_ReturnsCreated()
        {
            // Arrange: Create a Venue object to send to the API
            var venue = new Venue { VenueID = 5, VenueName = "TestVenue" };

            // Act: Send the POST request to the /api/Venue endpoint
            var response = await _client.PostAsJsonAsync("/api/Venue", venue);
            response.EnsureSuccessStatusCode();
            // Deserialize the returned content to check the saved venue
            var returned = await response.Content.ReadFromJsonAsync<Venue>();

            // Assert: Check if the returned venue is as expected
            Assert.Equal("TestVenue", returned.VenueName);
        }

        /// <summary>
        ///     testing get endpoint for getting a Venue
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the response code
        ///     Also if the returned name and Id is the same as expected
        /// </remark>
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
            Assert.Equal("Test Venue", returned.VenueName);
        }

        /// <summary>
        ///     Testing the put endpoint by first getting a Venue
        ///     Then use the enpoint to update its name
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the response code
        ///     Also if the returned name is updated as we expected
        /// </remark>
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

            var updatedVenue = await confirmResponse.Content.ReadFromJsonAsync<Venue>();

            Assert.Equal("Updated Venue Name", updatedVenue.VenueName);
        }

        /// <summary>
        ///     testing delete endpoint
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the response code
        ///     Also if the Venue can not be found after deletion
        /// </remark>
        [Fact]
        public async Task DeleteVenue_ReturnsDeletedVenue_And_CannotBeFoundAfter()
        {
            // Act
            var deleteResponse = await _client.DeleteAsync("/api/Venue/99");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getAfterDelete = await _client.GetAsync("/api/Venue/99");
            Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
        }

    }
}

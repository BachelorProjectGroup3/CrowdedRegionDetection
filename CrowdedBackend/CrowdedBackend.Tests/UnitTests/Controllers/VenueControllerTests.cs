using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CrowdedBackend.Controllers;
using CrowdedBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdedBackend.Tests.UnitTests.Controllers
{
    public class VenueControllerTests : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly MyDbContext _context;
        private readonly VenueController _controller;
        private readonly CustomWebApplicationFactory _factory;


        public VenueControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            // Use the factory to create a scope for the DB context
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new VenueController(_context);

        }

        /*
         * Post venue
         */

        [Fact]
        public async Task CreateVenue()
        {
            // Arrange
            var venue = new Venue
            {
                VenueID = 1,  // Assuming VenueID is the primary key
                VenueName = "Test Venue"
            };

            // Act
            var result = await _controller.PostVenue(venue);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result); // Assert it returns CreatedAtActionResult
            var returnedVenue = Assert.IsType<Venue>(createdAtActionResult.Value); // Assert the value is a Venue object

            Assert.Equal(venue.VenueID, returnedVenue.VenueID); // Check if the ID matches
            Assert.Equal(venue.VenueName, returnedVenue.VenueName); // Check if the name matches
        }

        /*
         * Get Venue by name
         */
        [Fact]
        public async Task GetVenue_ByName()
        {
            _context.Venue.Add(new Venue { VenueID = 2, VenueName = "Test Venue" });
            await _context.SaveChangesAsync();


            // Act
            var result = await _controller.GetVenue(2);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Venue>>(result);
            var venue = Assert.IsType<Venue>(actionResult.Value);
            Assert.Equal("Test Venue", venue.VenueName);
        }

        /*
         * Delete Venue by Id
         */
        [Fact]
        public async Task DeleteVenue_ById()
        {
            // Arrange: Add a venue to the in-memory database
            var venue = new Venue { VenueID = 3, VenueName = "Delete Venue" };
            _context.Venue.Add(venue);
            await _context.SaveChangesAsync(); // Save changes to the database

            // Act: Call DeleteVenue method
            var result = await _controller.DeleteVenue(3); // Pass the ID of the venue to delete

            // Assert: Verify the result is NoContent (204)
            Assert.IsType<NoContentResult>(result);

            // Assert: Check that the venue was removed from the database
            var deletedVenue = await _context.Venue.FindAsync(3);
            Assert.Null(deletedVenue); // Ensure the venue is no longer in the database
        }

        [Fact]
        public async Task DeleteVenue_VenueNotFound_ReturnsNotFound()
        {
            // Act: Try deleting a non-existing venue
            var result = await _controller.DeleteVenue(999); // Use a non-existing ID

            // Assert: Verify the result is NotFound (404)
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
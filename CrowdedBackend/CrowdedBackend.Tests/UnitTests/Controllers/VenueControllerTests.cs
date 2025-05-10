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
        public VenueControllerTests(CustomWebApplicationFactory factory)
        {
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new VenueController(_context);
        }

        /// <summary>
        ///     Creating a new Venue
        /// </summary>
        /// <remark>
        ///     Expected to pass because it should receive the same name and Id as expected
        /// </remark>
        [Fact]
        public async Task CreateVenue()
        {
            var venue = new Venue
            {
                VenueID = 1,
                VenueName = "Test Venue"
            };

            var result = await _controller.PostVenue(venue);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result); // Assert it returns CreatedAtActionResult
            var returnedVenue = Assert.IsType<Venue>(createdAtActionResult.Value); // Assert the value is a Venue object

            Assert.Equal(venue.VenueID, returnedVenue.VenueID); // Check if the ID matches
            Assert.Equal(venue.VenueName, returnedVenue.VenueName); // Check if the name matches
        }

        /// <summary>
        ///     Getting a venue by its name
        /// </summary>
        /// <remark>
        ///     Expected to pass because it should receive the same name as expected
        /// </remark>
        [Fact]
        public async Task GetVenue_ByName()
        {
            var result = await _controller.GetVenue(4);

            var actionResult = Assert.IsType<ActionResult<Venue>>(result);
            var venue = Assert.IsType<Venue>(actionResult.Value);
            Assert.Equal("Test Venue", venue.VenueName);
        }

        /// <summary>
        ///     Deleting a venue by its ID and trying to find that venue
        /// </summary>
        /// <remark>
        ///     Expected to return null because the venue should not exists
        /// </remark>
        [Fact]
        public async Task DeleteVenue_ById()
        {

            var result = await _controller.DeleteVenue(98);

            Assert.IsType<NoContentResult>(result);

            var deletedVenue = await _context.Venue.FindAsync(98);
            Assert.Null(deletedVenue);
        }

        [Fact]
        public async Task DeleteVenue_VenueNotFound_ReturnsNotFound()
        {

            var result = await _controller.DeleteVenue(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
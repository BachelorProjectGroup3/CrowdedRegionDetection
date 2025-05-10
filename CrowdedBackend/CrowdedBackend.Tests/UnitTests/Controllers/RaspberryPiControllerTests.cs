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
    public class RaspberryPiControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly MyDbContext _context;
        private readonly RaspberryPiController _controller;

        public RaspberryPiControllerTests(CustomWebApplicationFactory factory)
        {
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new RaspberryPiController(_context);
        }

        /// <summary>
        ///     Posting a RaspberryPi
        /// </summary>
        /// <remark>
        ///     Expected to pass by creating a Raspberry and checking Id And raspX value
        ///     Also checking if its has the correct type
        /// </remark>
        [Fact]
        public async Task PostRaspberryPi()
        {
            var raspberryPi = new RaspberryPi
            {
                VenueID = 2,
                RaspX = 40,
                RaspY = 70,
            };

            var result = await _controller.PostRaspberryPi(raspberryPi);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedVenue = Assert.IsType<RaspberryPi>(createdAtActionResult.Value);
            Assert.Equal(raspberryPi.RaspberryPiID, returnedVenue.RaspberryPiID);
            Assert.Equal(raspberryPi.RaspX, returnedVenue.RaspX);
        }
    }
}

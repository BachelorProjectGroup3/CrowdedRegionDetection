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
    public class RaspDataControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly MyDbContext _context;
        private readonly RaspDataController _controller;


        public RaspDataControllerTests(CustomWebApplicationFactory factory)
        {
            // Use the factory to create a scope for the DB context
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new RaspDataController(_context);
        }
        
        [Fact]
        public async Task CreateRaspData()
        {
            var raspData = new RaspData
            {
                MacAddress = "79:1C:89:6B:EC:C7",
                RaspId = 1,
                Rssi = -90,
                UnixTimestamp = 1746033900000
            };
            
            var result = await _controller.PostRaspData(raspData);
            
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedVenue = Assert.IsType<RaspData>(createdAtActionResult.Value);
            Assert.Equal(raspData.Id, returnedVenue.Id);
            Assert.Equal(raspData.MacAddress, returnedVenue.MacAddress);
        }

        /*
         * Get RaspData by name
         */
        [Fact]
        public async Task GetRaspData_ByName()
        {
            
            var result = await _controller.GetRaspData(998);
            
            var actionResult = Assert.IsType<ActionResult<RaspData>>(result);
            var raspData = Assert.IsType<RaspData>(actionResult.Value);
            Assert.Equal("79:1C:89:6B:EC:C7", raspData.MacAddress);
        }

        /*
         * Delete RaspData by Id
         */
        [Fact]
        public async Task DeleteRaspData_ById()
        {
            
            var result = await _controller.DeleteRaspData(999); // Pass the ID of the venue to delete
            
            Assert.IsType<NoContentResult>(result);
            var deletedRaspData = await _context.RaspData.FindAsync(3);
            Assert.Null(deletedRaspData); // Ensure the venue is no longer in the database
        }

        /*
         * Try to delete a non existing RaspData
         */
        [Fact]
        public async Task DeleteRaspData_RaspDataNotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteRaspData(999);
            
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

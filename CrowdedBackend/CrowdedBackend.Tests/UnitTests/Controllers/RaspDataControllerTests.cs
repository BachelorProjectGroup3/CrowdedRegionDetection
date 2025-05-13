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
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new RaspDataController(_context);
        }

        /// <summary>
        ///     Creating a RaspData 
        /// </summary>
        /// <remark>
        ///     Expected to pass by creating a RaspData and checking Id And MacAddress
        /// </remark>
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

        /// <summary>
        ///     Getting RaspData by its Id
        /// </summary>
        /// <remark>
        ///     Expected to pass by checking the received MacAddress and expected. Also checking if its a Type of RaspData 
        /// </remark>
        [Fact]
        public async Task GetRaspData_ById()
        {

            var result = await _controller.GetRaspData(998);

            var actionResult = Assert.IsType<ActionResult<RaspData>>(result);
            var raspData = Assert.IsType<RaspData>(actionResult.Value);
            Assert.Equal("79:1C:89:6B:EC:C7", raspData.MacAddress);
        }

        /// <summary>
        ///     Deleting a RaspData instance by its Id and trying to find it
        /// </summary>
        /// <remark>
        ///     Expected to delete a RaspData instance and after that trying to find it to make sure its deleted
        /// </remark>
        [Fact]
        public async Task DeleteRaspData_ById()
        {

            var result = await _controller.DeleteRaspData(999);
            Assert.IsType<NoContentResult>(result);

            var deletedRaspData = await _context.RaspData.FindAsync(999);
            Assert.Null(deletedRaspData);
        }

        /// <summary>
        ///     Trying to delete a non exisiting RaspData
        /// </summary>
        /// <remark>
        ///     Expected to fail because the RaspData instance should not be found
        /// </remark>
        [Fact]
        public async Task DeleteRaspData_RaspDataNotFound_ReturnsNotFound()
        {
            var result = await _controller.DeleteRaspData(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

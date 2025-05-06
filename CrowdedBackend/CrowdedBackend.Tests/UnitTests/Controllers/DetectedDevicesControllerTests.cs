using Xunit;
using CrowdedBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using CrowdedBackend.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace CrowdedBackend.Tests.UnitTests.Controllers
{
    public class DetectedDevicesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {

        /*private const long TimeInterval = 5 * 60 * 1000;
        private readonly MyDbContext _context;
        private readonly DetectedDevicesController _controller;
        private readonly CustomWebApplicationFactory _factory;

        public DetectedDevicesControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            // Use the factory to create a scope for the DB context
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            _controller = new DetectedDevicesController(_context);

        }*/
    }
}
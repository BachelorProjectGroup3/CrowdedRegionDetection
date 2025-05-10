using CrowdedBackend;
using CrowdedBackend.Models;
using CrowdedBackend.Services.CalculatePositions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdedBackend.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<CrowdedBackend.Program>
{
    private long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register CircleUtils in the DI container
            services.AddScoped<CircleUtils>();

            // Unique DB name per test class
            var dbName = $"TestDb_{GetType().Name}_{Guid.NewGuid()}";

            services.AddDbContext<MyDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            // Build and initialize DB
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            db.Database.EnsureDeleted(); // Clean slate
            db.Database.EnsureCreated();


            // Seed the database
            db.Venue.Add(new Venue { VenueID = 4, VenueName = "Test Venue" });
            db.Venue.Add(new Venue { VenueID = 99, VenueName = "Venue to Delete" });
            db.Venue.Add(new Venue { VenueID = 98, VenueName = "Second Venue to Delete" });

            db.RaspData.Add(new RaspData { Id = 998, MacAddress = "79:1C:89:6B:EC:C7", RaspId = 1, Rssi = -90, UnixTimestamp = 1746033900000 });
            db.RaspData.Add(new RaspData { Id = 999, MacAddress = "79:1C:89:6B:EC:C7", RaspId = 1, Rssi = -90, UnixTimestamp = 1746033900000 });

            db.DetectedDevice.Add(new DetectedDevice { DetectedDeviceId = 999, DeviceX = 50, DeviceY = 70, Timestamp = 1746535200000, VenueID = 4 });
            db.DetectedDevice.Add(new DetectedDevice { DetectedDeviceId = 998, DeviceX = 60, DeviceY = 90, Timestamp = 1746535200000, VenueID = 4 });
            db.DetectedDevice.Add(new DetectedDevice { DetectedDeviceId = 997, DeviceX = 80, DeviceY = 40, Timestamp = 1746535200000, VenueID = 4 });

            // Adding Raspberry Pi devices to the database
            db.RaspberryPi.Add(new RaspberryPi { VenueID = 10, RaspX = 50, RaspY = 60 });
            db.RaspberryPi.Add(new RaspberryPi { VenueID = 10, RaspX = 90, RaspY = 100 });
            db.RaspberryPi.Add(new RaspberryPi { VenueID = 10, RaspX = 50, RaspY = 30 });

            db.SaveChanges();
        });
    }
}
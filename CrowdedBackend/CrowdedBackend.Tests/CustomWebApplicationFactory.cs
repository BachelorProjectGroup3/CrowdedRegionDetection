using CrowdedBackend;
using CrowdedBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdedBackend.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<CrowdedBackend.Program>
{
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

            // Unique DB name per test class
            var dbName = $"TestDb_{GetType().Name}";

            services.AddDbContext<MyDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
            
            // Build and initialize DB
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            db.Database.EnsureDeleted(); // Clean slate
            db.Database.EnsureCreated();
        });
    }
}
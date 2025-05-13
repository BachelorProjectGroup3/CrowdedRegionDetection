using CrowdedBackend.Hubs;
using CrowdedBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddOpenApi();

// Conditionally configure the database based on the environment
var env = builder.Environment.EnvironmentName;
if (env == "Testing")
{
    // Use InMemory database for testing
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    // Use PostgreSQL in development or production environments
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Referer");
    logging.ResponseHeaders.Add("MyCustomResponseHeader");
});


builder.Services.AddSignalR();


builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<DetectedDeviceHub>("/hubs/detecteddevices");

app.MapHub<DetectedDeviceHub>("/hubs/detecteddevices");

app.UseHttpLogging();

app.UseHttpsRedirection();

app.MapControllers(); // Add this to map controllers (including your RaspDataController)

app.Run();
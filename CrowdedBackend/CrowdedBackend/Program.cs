using CrowdedBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add this to use controllers
builder.Services.AddOpenApi(); // Optional, for OpenAPI/Swagger documentation

// Configure PostgreSQL database connection
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Optional, for OpenAPI/Swagger in development
}

app.UseHttpsRedirection();

app.MapControllers(); // Add this to map controllers (including your RaspDataController)

app.Run();
using Microsoft.EntityFrameworkCore;
using CrowdedBackend.Models;

namespace CrowdedBackend.Models;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Venue> Venue { get; set; } = default!;
    public DbSet<RaspberryPi> RaspberryPi { get; set; } = default!;
    public DbSet<DetectedDevice> DetectedDevice { get; set; } = default!;
    public DbSet<RaspData> RaspData { get; set; } = default!;
}

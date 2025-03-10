using Microsoft.EntityFrameworkCore;

namespace CrowdedBackend.Models;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } // Example entity

    public DbSet<DefaultNamespace.RaspData> RaspData { get; set; } = default!;
}

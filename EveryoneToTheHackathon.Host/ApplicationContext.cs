using EveryoneToTheHackathon.DataContracts;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Host;

public sealed class ApplicationContext : DbContext
{
    private string _connectionString;

    public DbSet<Hackathon> Hackathons { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    
    public ApplicationContext(string connectionString)
    {
        _connectionString = connectionString;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hackathon>();
        modelBuilder.Entity<Employee>();
        modelBuilder.Entity<Wishlist>();
        modelBuilder.Entity<Team>();
        modelBuilder.Ignore<HRDirector>();
        modelBuilder.Ignore<HRManager>();
        modelBuilder.Ignore<ProposeAndRejectAlgorithm>();
        
    }
}
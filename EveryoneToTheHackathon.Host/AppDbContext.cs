using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Host;

public sealed class AppDbContext : DbContext
{
    //private readonly string _connectionString;

    public DbSet<Hackathon> Hackathons { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)//string connectionString)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hackathon>().HasMany(h => h.Employees).WithOne(e => e.Hackathon);
        modelBuilder.Entity<Hackathon>().HasMany(h => h.Teams).WithOne(t => t.Hackathon);

        modelBuilder.Entity<Employee>().HasOne(e => e.Hackathon).WithMany(h => h.Employees).HasForeignKey(e => e.HackathonId);
        modelBuilder.Entity<Employee>().HasOne(e => e.Team).WithMany(t => t.Employees).HasForeignKey(e => e.TeamId);
        
        modelBuilder.Entity<Wishlist>().HasOne(w => w.Employee).WithOne(e => e.Wishlist).HasForeignKey<Wishlist>(w => new { w.EmployeeId, w.EmployeeTitle });

        modelBuilder.Entity<Team>().HasMany(t => t.Employees).WithOne(e => e.Team);
        modelBuilder.Entity<Team>().HasOne(t => t.Hackathon).WithMany(h => h.Teams).HasForeignKey(t => t.HackathonId);
        
        modelBuilder.Ignore<HRDirector>();
        modelBuilder.Ignore<HRManager>();
        modelBuilder.Ignore<ProposeAndRejectAlgorithm>();
        
    }
}
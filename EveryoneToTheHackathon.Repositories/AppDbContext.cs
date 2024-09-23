using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public sealed class AppDbContext : DbContext
{
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Employee> Employees { get; set; } 
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hackathon>().HasMany(h => h.Employees).WithMany(e => e.Hackathons);
        modelBuilder.Entity<Hackathon>().HasMany(h => h.Wishlists).WithOne(w => w.Hackathon);
        modelBuilder.Entity<Hackathon>().HasMany(h => h.Teams).WithOne(t => t.Hackathon);

        modelBuilder.Entity<Employee>().HasMany(e => e.Hackathons).WithMany(h => h.Employees);
        modelBuilder.Entity<Employee>().HasMany(e => e.Teams).WithMany(t => t.Employees);//./*HasForeignKey(e => e.TeamId).*/IsRequired(false);
        
        modelBuilder.Entity<Wishlist>().HasOne(w => w.Employee).WithMany(e => e.Wishlists).HasForeignKey(w => new { w.EmployeeId, w.EmployeeTitle }).OnDelete(DeleteBehavior.Cascade).IsRequired(true);
        modelBuilder.Entity<Wishlist>().HasOne(w => w.Hackathon).WithMany(h => h.Wishlists).HasForeignKey(w => w.HackathonId).OnDelete(DeleteBehavior.Cascade).IsRequired(true);
        
        modelBuilder.Entity<Team>().HasMany(t => t.Employees).WithMany(e => e.Teams);//.WithOne(e => e.Team);
        modelBuilder.Entity<Team>().HasOne(t => t.Hackathon).WithMany(h => h.Teams).HasForeignKey(t => t.HackathonId).OnDelete(DeleteBehavior.Cascade).IsRequired(true);
        
        modelBuilder.Ignore<HRDirector>();
        modelBuilder.Ignore<HRManager>();
        modelBuilder.Ignore<ProposeAndRejectAlgorithm>();
        
    }
}
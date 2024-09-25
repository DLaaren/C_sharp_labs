using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Tests;

public class DatabaseTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        return new AppDbContext(options);
    }
    
    [Fact]
    private void TestingDatabase()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var hackathonService = new HackathonRepository(dbContext);
        var hackathon = new Hackathon(
            fixture.TeamLeads, 
            fixture.Juniors, 
            new HRManager(new ProposeAndRejectAlgorithm()), 
            new HRDirector())
        {
            // Act
            Id = 1,
            MeanSatisfactionIndex = 4.2,
            Employees = fixture.TeamLeads.Concat(fixture.Juniors).ToList(),
            Wishlists = fixture.TeamLeadsWishlists.Concat(fixture.JuniorsWishlists).ToList(),
            Teams = fixture.Teams
        };

        hackathonService.AddHackathon(hackathon);
        
        // Assert
        var hackathonResult = dbContext.Hackathons.
            Include(h => h.Employees).
            Include(h => h.Wishlists).
            Include(h => h.Teams).
            Single(h => h.Equals(hackathon));
        
        Assert.NotNull(hackathonResult);
        Assert.True(hackathonResult.Id > 0);
        Assert.Equal(4.2, hackathonResult.MeanSatisfactionIndex);
        Assert.Equal(hackathon.Employees, hackathonResult.Employees!);
        Assert.Equal(hackathon.Wishlists, hackathonResult.Wishlists!);
        Assert.Equal(hackathon.Teams, hackathonResult.Teams!);
        
        // Clean up
        hackathonService.DeleteHackathon(hackathon.Id);
    }
    
    
    [Fact]
    private void ReadingAndWritingMeanSatisfactionIndexFromDatabase()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var hackathonService = new HackathonRepository(dbContext);
        var hackathon1 = new Hackathon
        {
            Id = 2,
            MeanSatisfactionIndex = 1.2
        };
        var hackathon2 = new Hackathon
        {
            Id = 3,
            MeanSatisfactionIndex = 4.2
        };

        // Act
        hackathonService.AddHackathon(hackathon1);
        hackathonService.AddHackathon(hackathon2);
        
        // Assert
        var hackathonResult1 = dbContext.Hackathons.
            Single(h => h.Equals(hackathon1));
        var hackathonResult2 = dbContext.Hackathons.
            Single(h => h.Equals(hackathon2));
        
        Assert.NotNull(hackathonResult1);
        Assert.NotNull(hackathonResult2);
        Assert.Equal(1.2, hackathonResult1.MeanSatisfactionIndex);
        Assert.Equal(4.2, hackathonResult2.MeanSatisfactionIndex);
        Assert.Equal(2.7, hackathonService.GetMeanSatisfactionIndexForAllRounds());
    }
}
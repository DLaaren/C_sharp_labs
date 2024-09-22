using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using EveryoneToTheHackathon.Services;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace EveryoneToTheHackathon.Tests;

public class DatabaseTests
{
    private AppDbContext GetInMemoryDbContext()
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
        var teamLeads = new List<Employee>
        {
            new Employee(1, EmployeeTitle.TeamLead, "John Doe"),
            new Employee(2, EmployeeTitle.TeamLead, "Jane Black"),
            new Employee(3, EmployeeTitle.TeamLead, "Bob Richman"),
            new Employee(4, EmployeeTitle.TeamLead, "Aboba Abobovich"),
            new Employee(5, EmployeeTitle.TeamLead, "Chuck Norris")
        };
        var juniors = new List<Employee>
        {
            new Employee(1, EmployeeTitle.Junior, "Walter White"),
            new Employee(2, EmployeeTitle.Junior, "Arnold Kindman"),
            new Employee(3, EmployeeTitle.Junior, "Jack Jones"),
            new Employee(4, EmployeeTitle.Junior, "Jane Jordan"),
            new Employee(5, EmployeeTitle.Junior, "Ken Kennedy")
        };
        
        var teamLeadsWishlists = new List<Wishlist>(5);
        var juniorsWishlists = new List<Wishlist>(5);
        for (var i = 1; i <= teamLeads.Count; i++)
        {
            Random seed = new Random(i);
            teamLeadsWishlists.Add(new Wishlist(i, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }
        for (var i = 1; i <= juniors.Count; i++)
        {
            Random seed = new Random(i * 100);
            juniorsWishlists.Add(new Wishlist(i, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }
        
        List<Team> teams = new List<Team>
        {
            new Team(new Employee(1, EmployeeTitle.TeamLead,"John Doe"), new Employee(4, EmployeeTitle.Junior,"Jane Jordan")),
            new Team(new Employee(5, EmployeeTitle.TeamLead,"Chuck Norris"), new Employee(3, EmployeeTitle.Junior,"Jack Jones")),
            new Team(new Employee(2, EmployeeTitle.TeamLead,"Jane Black"), new Employee(1, EmployeeTitle.Junior,"Walter White")),
            new Team(new Employee(3, EmployeeTitle.TeamLead,"Bob Richman"), new Employee(2, EmployeeTitle.Junior,"Arnold Kindman")),
            new Team(new Employee(4, EmployeeTitle.TeamLead,"Aboba Abobovich"), new Employee(5, EmployeeTitle.Junior,"Ken Kennedy"))
        };
        
        var dbContext = GetInMemoryDbContext();
        var hackathonService = new HackathonService(dbContext);
        var hackathon = new Hackathon(
            teamLeads, 
            juniors, 
            new HRManager(new ProposeAndRejectAlgorithm()), 
            new HRDirector());
        
        // Act
        hackathon.Id = 1;
        hackathon.MeanSatisfactionIndex = 4.2;
        hackathon.Employees = teamLeads.Concat(juniors).ToList();
        hackathon.Wishlists = teamLeadsWishlists.Concat(juniorsWishlists).ToList();
        hackathon.Teams = teams;
        
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
        Assert.Equal(hackathon.Employees, hackathonResult.Employees);
        Assert.Equal(hackathon.Wishlists, hackathonResult.Wishlists);
        Assert.Equal(hackathon.Teams, hackathonResult.Teams);
        
        // Clean up
        hackathonService.DeleteHackathon(hackathon.Id);
    }
    
    
    [Fact]
    private void ReadingAndWritingMeanSatisfactionIndexFromDatabase()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var hackathonService = new HackathonService(dbContext);
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
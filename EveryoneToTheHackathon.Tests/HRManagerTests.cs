using System.Reflection;
using EveryoneToTheHackathon.Entities;
using Moq;
using Xunit.Abstractions;

namespace EveryoneToTheHackathon.Tests;

public class HRManagerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HRManagerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CheckStrategyResultsWithDefinedData()
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
        HRManager hrManager = new HRManager(new ProposeAndRejectAlgorithm());

        List<Team> resultTeams = new List<Team>
        {
            new Team(teamLeads[0], juniors[1]),
            new Team(teamLeads[2], juniors[3]),
            new Team(teamLeads[3], juniors[4]),
            new Team(teamLeads[4], juniors[2]),
            new Team(teamLeads[1], juniors[0])
        };

        // Act
        List<Team> teams = (List<Team>)hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
        
        // Assert
        Assert.Equal(teams, resultTeams);
    }

    [Fact]
    public void CheckNumberOfStrategyCalls()
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

        var algoMock = new Mock<ITeamBuildingStrategy>();
        var hrManager = new HRManager(algoMock.Object);
        
        // Act
        var teams = hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
        
        // Assert
        algoMock.Verify(a => a.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists), Times.Once);
    }
}
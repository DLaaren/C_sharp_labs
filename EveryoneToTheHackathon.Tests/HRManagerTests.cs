using EveryoneToTheHackathon.Entities;
using Moq;

namespace EveryoneToTheHackathon.Tests;

public class HRManagerTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void CheckStrategyResultsWithDefinedData()
    {
        // Arrange
        HRManager hrManager = new HRManager(new ProposeAndRejectAlgorithm());
        
        // Act
        List<Team> teams = (List<Team>)hrManager.BuildTeams(fixture.TeamLeads, fixture.Juniors, fixture.TeamLeadsWishlists, fixture.JuniorsWishlists);
        
        // Assert
        Assert.Equal(teams.Select(t => t.TeamLead.Id), fixture.Teams.Select(t => t.TeamLead.Id));
        Assert.Equal(teams.Select(t => t.TeamLead.Title), fixture.Teams.Select(t => t.TeamLead.Title));
        Assert.Equal(teams.Select(t => t.TeamLead.Name), fixture.Teams.Select(t => t.TeamLead.Name));
        Assert.Equal(teams.Select(t => t.Junior.Id), fixture.Teams.Select(t => t.Junior.Id));
        Assert.Equal(teams.Select(t => t.Junior.Title), fixture.Teams.Select(t => t.Junior.Title));
        Assert.Equal(teams.Select(t => t.Junior.Name), fixture.Teams.Select(t => t.Junior.Name));
    }

    [Fact]
    public void CheckNumberOfStrategyCalls()
    {
        // Arrange
        var algoMock = new Mock<ITeamBuildingStrategy>();
        var hrManager = new HRManager(algoMock.Object);
        
        // Act
        var teams = hrManager.BuildTeams(fixture.TeamLeads, fixture.Juniors, fixture.TeamLeadsWishlists, fixture.JuniorsWishlists);
        
        // Assert
        algoMock.Verify(a => a.BuildTeams(fixture.TeamLeads, fixture.Juniors, fixture.TeamLeadsWishlists, fixture.JuniorsWishlists), Times.Once);
    }
}
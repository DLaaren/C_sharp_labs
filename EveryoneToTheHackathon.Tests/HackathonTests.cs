using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Tests;

public class HackathonTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void CheckHackathonResultWithDefinedData()
    {
        // Arrange
        Hackathon hackathon = new Hackathon(
            fixture.TeamLeads, 
            fixture.Juniors,
            new HRManager(new ProposeAndRejectAlgorithm()),
            new HRDirector()
            );
        
        // Act = perform test
        hackathon.HoldEvent(fixture.TeamLeadsWishlists, fixture.JuniorsWishlists);

        // Assert = validate test's results
        Assert.Equal(4.2, hackathon.MeanSatisfactionIndex);
    }
}
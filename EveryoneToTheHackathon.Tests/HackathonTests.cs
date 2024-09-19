using EveryoneToTheHackathon.DataContracts;
using Xunit.Abstractions;

namespace EveryoneToTheHackathon.Tests;

public class HackathonTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HackathonTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CheckHackathonResultWithDefinedData()
    {
        // Arrange
        var teamLeads = new List<Employee>
        {
            new Employee(1, "John Doe"),
            new Employee(2, "Jane Black"),
            new Employee(3, "Bob Richman"),
            new Employee(4, "Aboba Abobovich"),
            new Employee(5, "Chuck Norris")
        };
        var juniors = new List<Employee>
        {
            new Employee(1, "Walter White"),
            new Employee(2, "Arnold Kindman"),
            new Employee(3, "Jack Jones"),
            new Employee(4, "Jane Jordan"),
            new Employee(5, "Ken Kennedy")
        };

        var teamLeadsWishlists = new List<Wishlist>(5);
        var juniorsWishlists = new List<Wishlist>(5);
        for (var i = 1; i <= teamLeads.Count; i++)
        {
            Random seed = new Random(i);
            teamLeadsWishlists.Add(new Wishlist(i, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }
        for (var i = 1; i <= juniors.Count; i++)
        {
            Random seed = new Random(i * 100);
            juniorsWishlists.Add(new Wishlist(i, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }

        Hackathon hackathon = new Hackathon(
            teamLeads, 5, 
            juniors, 5,
            new HRManager(new ProposeAndRejectAlgorithm()), new HRDirector()
            );
        
        // Act = perform test
        hackathon.HoldEvent(teamLeadsWishlists, juniorsWishlists);

        // Assert = validate test's results
        Assert.Equal(4.2, hackathon.MeanSatisfactionIndex);
    }
}
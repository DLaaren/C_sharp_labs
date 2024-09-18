using EveryoneToTheHackathon.DataContracts;

namespace EveryoneToTheHackathon.Tests;

public class WishlistTests
{
    [Fact]
    public void WishlistSizeEqualsAmountOfPossibleTeammates()
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
        
        // Act
        var teamLeadsWishlists = teamLeads.Select(teamlead => teamlead.MakeWishlist(juniors)).ToList();
        var juniorsWishlists = juniors.Select(junior => junior.MakeWishlist(teamLeads)).ToList();
        
        // Assert
        teamLeadsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.Length, juniors.Count));
        juniorsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.Length, teamLeads.Count));
    }

    [Fact]
    public void WishlistContainsPossibleTeammates()
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
        
        // Act
        var teamLeadsWishlists = teamLeads.Select(teamlead => teamlead.MakeWishlist(juniors)).ToList();
        var juniorsWishlists = juniors.Select(junior => junior.MakeWishlist(teamLeads)).ToList();
        
        // Assert
        teamLeadsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
        juniorsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
    }
}
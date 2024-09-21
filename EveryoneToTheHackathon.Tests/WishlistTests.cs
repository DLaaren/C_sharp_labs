using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Tests;

public class WishlistTests
{
    [Fact]
    public void WishlistSizeEqualsAmountOfPossibleTeammates()
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

        
        // Act
        var teamLeadsWishlists = teamLeads.Select(teamlead => teamlead.MakeWishlist(juniors)).ToList();
        var juniorsWishlists = juniors.Select(junior => junior.MakeWishlist(teamLeads)).ToList();
        
        // Assert
        teamLeadsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
        juniorsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
    }
}
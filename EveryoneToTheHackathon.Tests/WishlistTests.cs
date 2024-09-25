using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Tests;

// create test base 

public class WishlistTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void WishlistSizeEqualsAmountOfPossibleTeammates()
    {
        // Act
        var teamLeadsWishlists = fixture.TeamLeads.Select(teamlead => teamlead.MakeWishlist(fixture.Juniors)).ToList();
        var juniorsWishlists = fixture.Juniors.Select(junior => junior.MakeWishlist(fixture.TeamLeads)).ToList();
        
        // Assert
        teamLeadsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.Length, fixture.Juniors.Count));
        juniorsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.Length, fixture.TeamLeads.Count));
    }

    [Fact]
    public void WishlistContainsPossibleTeammates()
    {
        // Act
        var teamLeadsWishlists = fixture.TeamLeads.Select(teamlead => teamlead.MakeWishlist(fixture.Juniors)).ToList();
        var juniorsWishlists = fixture.Juniors.Select(junior => junior.MakeWishlist(fixture.TeamLeads)).ToList();
        
        // Assert
        teamLeadsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
        juniorsWishlists.ForEach(wishlist => Assert.Equal(wishlist.DesiredEmployees.OrderDescending(), Enumerable.Range(1, 5).OrderDescending()));
    }
}
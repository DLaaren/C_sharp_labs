namespace lab1;

public class HRDirector
{
    public double CalculateMeanSatisfactionIndex(
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists,
        IEnumerable<Team> teams)
    {
        var teamsList = (List<Team>)teams;
        var teamLeadsWishlistsList = (List<Wishlist>)teamLeadsWishlists;
        var juniorsWishlistsList = (List<Wishlist>)juniorsWishlists;
        
        int[] juniorsSatisfactionIndexes = new int[teamsList.Count];
        int[] teamLeadsSatisfactionIndexes = new int[teamsList.Count];
        
        foreach (var team in teams)
        {
            var teamLead = team.TeamLead;
            var junior = team.Junior;

            var teamLeadWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == teamLead.Id);
            var juniorWishlist = juniorsWishlistsList.Find(w => w.EmployeeId == junior.Id);

            var teamLeadSatisfactionIndex = 20 - Array.FindIndex(teamLeadWishlist.DesiredEmployees, j => j == junior.Id);
            var juniorSatisfactionIndex = 20 - Array.FindIndex(juniorWishlist.DesiredEmployees, t => t == teamLead.Id);

            juniorsSatisfactionIndexes[junior.Id - 1] = juniorSatisfactionIndex;
            teamLeadsSatisfactionIndexes[teamLead.Id - 1] = teamLeadSatisfactionIndex;
        }
        
        return (double)(juniorsSatisfactionIndexes.Sum() + teamLeadsSatisfactionIndexes.Sum()) / (juniorsSatisfactionIndexes.Length + teamLeadsSatisfactionIndexes.Length);
    }
}
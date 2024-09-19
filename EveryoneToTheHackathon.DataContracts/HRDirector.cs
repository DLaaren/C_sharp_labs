using System.Diagnostics;

namespace EveryoneToTheHackathon.DataContracts;

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
            Employee teamLead = team.TeamLead;
            Employee junior = team.Junior;

            int[]? teamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == teamLead.Id)?.DesiredEmployees;
            Debug.Assert(teamLeadWishlistIds != null, nameof(teamLeadWishlistIds) + " != null");
            int[]? juniorWishlistIds = juniorsWishlistsList.Find(w => w.EmployeeId == junior.Id)?.DesiredEmployees;
            Debug.Assert(juniorWishlistIds != null, nameof(juniorWishlistIds) + " != null");
            
            var teamLeadSatisfactionIndex = teamLeadsWishlistsList.Count - Array.FindIndex(teamLeadWishlistIds, j => j == junior.Id);
            var juniorSatisfactionIndex = juniorsWishlistsList.Count - Array.FindIndex(juniorWishlistIds, t => t == teamLead.Id);

            juniorsSatisfactionIndexes[junior.Id - 1] = juniorSatisfactionIndex;
            teamLeadsSatisfactionIndexes[teamLead.Id - 1] = teamLeadSatisfactionIndex;
        }
        
        return CalculateMean(juniorsSatisfactionIndexes.Concat(teamLeadsSatisfactionIndexes), juniorsSatisfactionIndexes.Length + teamLeadsSatisfactionIndexes.Length);
    }

    private double CalculateMean(IEnumerable<int> numbers, int count)
    {
        return (double)numbers.Sum() / count;
    }
}
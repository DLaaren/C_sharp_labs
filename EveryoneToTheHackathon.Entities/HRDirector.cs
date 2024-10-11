using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveryoneToTheHackathon.Entities;

[NotMapped]
public class HRDirector
{
    public double CalculateMeanSatisfactionIndex(
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists,
        IEnumerable<Team> teams)
    {
        var teamsList = (List<Team>)teams;
        var teamLeadsWishlistsList = (List<Wishlist>)teamLeadsWishlists;
        var juniorsWishlistsList = (List<Wishlist>)juniorsWishlists;
        
        var juniorsSatisfactionIndexes = new int[teamsList.Count];
        var teamLeadsSatisfactionIndexes = new int[teamsList.Count];
        
        foreach (var team in teams)
        {
            var teamLead = team.TeamLead;
            var junior = team.Junior;
            Debug.Assert(teamLead != null, "Team does not contain a team lead");
            Debug.Assert(junior != null, "Team does not contain a junior");

            var teamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == teamLead.Id)?.DesiredEmployees;
            Debug.Assert(teamLeadWishlistIds != null);
            var juniorWishlistIds = juniorsWishlistsList.Find(w => w.EmployeeId == junior.Id)?.DesiredEmployees;
            Debug.Assert(juniorWishlistIds != null);
            
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
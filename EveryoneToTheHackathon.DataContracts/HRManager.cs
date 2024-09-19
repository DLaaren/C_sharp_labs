using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveryoneToTheHackathon.DataContracts;

[NotMapped]
public class HRManager(ITeamBuildingStrategy strategy)
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, 
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        Debug.Assert(((List<Employee>)teamLeads).Count == ((List<Employee>)juniors).Count);
        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}
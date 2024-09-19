using System.Diagnostics;

namespace EveryoneToTheHackathon.DataContracts;

public class HRManager 
{
    private readonly ITeamBuildingStrategy _strategy;

    public HRManager(ITeamBuildingStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, 
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        Debug.Assert(((List<Employee>)teamLeads).Count == ((List<Employee>)juniors).Count);
        return _strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}
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
        return _strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}
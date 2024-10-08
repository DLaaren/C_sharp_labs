using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveryoneToTheHackathon.Entities;

public class Hackathon
{
    /* Public properties for database */
    [Key]
    public int Id { get; set; }
    public double MeanSatisfactionIndex { get; set; }
    /* Public properties for database */

    /* Navigation properties */
    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    public IEnumerable<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public IEnumerable<Team> Teams { get; set; } = new List<Team>();
    /* Navigation properties */
    
    /* Private properties for inner logic */
    private readonly List<Employee> _teamLeads;
    private readonly List<Employee> _juniors;
    
    private readonly HRManager _hrManager;
    private readonly HRDirector _hrDirector;

    private List<Wishlist>? _juniorsWishlists;
    private List<Wishlist>? _teamLeadsWishlists;
    /* Private properties for inner logic */

    public Hackathon() {}

    public Hackathon(
        IEnumerable<Employee> teamLeads,
        IEnumerable<Employee> juniors,
        HRManager hrManager, HRDirector hrDirector)
    {
        _teamLeads = (List<Employee>)teamLeads;
        _juniors = (List<Employee>)juniors;
        _hrManager = hrManager;
        _hrDirector = hrDirector;

        ((List<Employee>)Employees).AddRange(teamLeads);
        ((List<Employee>)Employees).AddRange(juniors);
    }

    public void HoldEvent()
    {
        _teamLeadsWishlists = _teamLeads.Select(teamlead => teamlead.MakeWishlist(_juniors)).ToList();
        _juniorsWishlists = _juniors.Select(junior => junior.MakeWishlist(_teamLeads)).ToList();

        Wishlists = _teamLeadsWishlists.Concat(_juniorsWishlists).ToList();
        
        Teams = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        MeanSatisfactionIndex = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, Teams);
        
        ((List<Team>)Teams).ForEach(t =>
        {
            t.Hackathon = this;
            t.HackathonId = Id;
        });
        
        ((List<Employee>)Employees!).ForEach(e => ((List<Team>)e.Teams!).AddRange( ((List<Team>)Teams).FindAll(t => t.TeamLead.Equals(e) || t.Junior.Equals(e))) );
    }
    
    public void HoldEvent(IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        _teamLeadsWishlists = (List<Wishlist>)teamLeadsWishlists;
        _juniorsWishlists = (List<Wishlist>)juniorsWishlists;
        
        Wishlists = _teamLeadsWishlists.Concat(_juniorsWishlists).ToList();

        Teams = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        MeanSatisfactionIndex = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, Teams);
        
        ((List<Team>)Teams).ForEach(t =>
        {
            t.Hackathon = this;
            t.HackathonId = Id;
        });
        
        ((List<Employee>)Employees!).ForEach(e => ((List<Team>)e.Teams!).AddRange( ((List<Team>)Teams).FindAll(t => t.TeamLead.Equals(e) || t.Junior.Equals(e))) );
    }
}
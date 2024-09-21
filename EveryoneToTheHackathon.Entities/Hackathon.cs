using System.ComponentModel.DataAnnotations;

namespace EveryoneToTheHackathon.Entities;

public class Hackathon : IHackathon
{
    [Key]
    public int Id { get; init; }
    [Required]
    public double MeanSatisfactionIndex { get; private set; }

    public List<Employee>? Employees { get; set; }
    public List<Team>? Teams { get; set; }

    private List<Employee> _teamLeads;
    private int _teamLeadsNumber;
    private List<Employee> _juniors;
    private int _juniorsNumber;
    
    private readonly HRManager _hrManager;
    private readonly HRDirector _hrDirector;
    
    private List<Wishlist>? _juniorsWishlists;
    private List<Wishlist>? _teamLeadsWishlists;

    public Hackathon() {}
    
    public Hackathon(
        IEnumerable<Employee> teamLeads, int teamLeadsNumber, 
        IEnumerable<Employee> juniors, int juniorsNumber, 
        HRManager hrManager, HRDirector hrDirector)
    {
        _teamLeads = (List<Employee>)teamLeads; 
        _teamLeadsNumber = teamLeadsNumber;
        _juniors = (List<Employee>)juniors;
        _juniorsNumber = juniorsNumber;
        _hrManager = hrManager;
        _hrDirector = hrDirector;
    }

    public void HoldEvent()
    {
        _teamLeadsWishlists = _teamLeads.Select(teamlead => teamlead.MakeWishlist(_juniors)).ToList();
        _juniorsWishlists = _juniors.Select(junior => junior.MakeWishlist(_teamLeads)).ToList();

        var teams1 = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        double idx1 = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, teams1);
        
        var teams2 = (List<Team>)_hrManager.BuildTeams(_juniors, _teamLeads, _juniorsWishlists, _teamLeadsWishlists);
        double idx2 = _hrDirector.CalculateMeanSatisfactionIndex(_juniorsWishlists, _teamLeadsWishlists, teams2);
        
        Teams = idx1 > idx2 ? teams1 : teams2;
        MeanSatisfactionIndex = idx1 > idx2 ? idx1 : idx2;
    }
    
    public void HoldEvent(IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        _teamLeadsWishlists = (List<Wishlist>)teamLeadsWishlists;
        _juniorsWishlists = (List<Wishlist>)juniorsWishlists;

        var teams1 = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        double idx1 = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, teams1);
        
        var teams2 = (List<Team>)_hrManager.BuildTeams(_juniors, _teamLeads, _juniorsWishlists, _teamLeadsWishlists);
        double idx2 = _hrDirector.CalculateMeanSatisfactionIndex(_juniorsWishlists, _teamLeadsWishlists, teams2);
        
        Teams = idx1 > idx2 ? teams1 : teams2;
        MeanSatisfactionIndex = idx1 > idx2 ? idx1 : idx2;
    }
}
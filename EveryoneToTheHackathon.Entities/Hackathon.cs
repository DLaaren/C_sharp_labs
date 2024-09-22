using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveryoneToTheHackathon.Entities;

public class Hackathon : IHackathon
{
    [Key]
    public int Id { get; set; }
    public double MeanSatisfactionIndex { get; set; }

    public List<Team>? Teams { get; set; }
    public List<Employee>? Employees { get; set; }
    public List<Wishlist>? Wishlists { get; set; }
    
    private readonly List<Employee> _teamLeads;
    private readonly List<Employee> _juniors;
    
    private readonly HRManager _hrManager;
    private readonly HRDirector _hrDirector;

    private List<Wishlist>? _juniorsWishlists;
    private List<Wishlist>? _teamLeadsWishlists;

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

        Employees = new List<Employee>();
        Employees.AddRange(teamLeads);
        Employees.AddRange(juniors);
    }

    public void HoldEvent()
    {
        _teamLeadsWishlists = _teamLeads.Select(teamlead => teamlead.MakeWishlist(_juniors)).ToList();
        _juniorsWishlists = _juniors.Select(junior => junior.MakeWishlist(_teamLeads)).ToList();

        Wishlists = _teamLeadsWishlists.Concat(_juniorsWishlists).ToList();
        
        var teams1 = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        double idx1 = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, teams1);
        
        var teams2 = (List<Team>)_hrManager.BuildTeams(_juniors, _teamLeads, _juniorsWishlists, _teamLeadsWishlists);
        double idx2 = _hrDirector.CalculateMeanSatisfactionIndex(_juniorsWishlists, _teamLeadsWishlists, teams2);
        
        Teams = idx1 > idx2 ? teams1 : teams2;
        Teams.ForEach(t =>
        {
            t.Hackathon = this;
            t.HackathonId = Id;
        });
        MeanSatisfactionIndex = idx1 > idx2 ? idx1 : idx2;
    }
    
    public void HoldEvent(IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        _teamLeadsWishlists = (List<Wishlist>)teamLeadsWishlists;
        _juniorsWishlists = (List<Wishlist>)juniorsWishlists;
        
        Wishlists = _teamLeadsWishlists.Concat(_juniorsWishlists).ToList();

        var teams1 = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        double idx1 = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, teams1);
        
        var teams2 = (List<Team>)_hrManager.BuildTeams(_juniors, _teamLeads, _juniorsWishlists, _teamLeadsWishlists);
        double idx2 = _hrDirector.CalculateMeanSatisfactionIndex(_juniorsWishlists, _teamLeadsWishlists, teams2);
        
        Teams = idx1 > idx2 ? teams1 : teams2;
        Teams.ForEach(t =>
        {
            t.Hackathon = this;
            t.HackathonId = Id;
        });
        MeanSatisfactionIndex = idx1 > idx2 ? idx1 : idx2;
    }
}
using System.Collections.Concurrent;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorService(IOptions<ServiceSettings> settings, HRDirector hrDirector, IHackathonRepository hackathonRepository)
{
    public readonly int EmployeesNumber = settings.Value.EmployeesNumber;
    private HRDirector HrDirector { get; } = hrDirector;
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;

    public ConcurrentBag<Employee> Employees { get; set; } = [];
    //public List<Employee>? Employees { get; set; }
    //public List<Wishlist>? Wishlists { get; set; }
    public ConcurrentBag<Wishlist> Wishlists { get; set; } = [];
    public List<Team> Teams { get; set; } = [];
    
    public readonly TaskCompletionSource<bool> EmployeesGotTcs = new();
    public readonly TaskCompletionSource<bool> WishlistsGotTcs = new();
    public readonly TaskCompletionSource<bool> TeamsGotTcs = new();
    
    public double CalculationMeanSatisfactionIndex()
    {
        var teamleadsWishlists = Wishlists.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)).ToList();
        var juniorsWishlists = Wishlists.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)).ToList();
        // var teams = Teams.ToList();
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(teamleadsWishlists, juniorsWishlists, Teams);
        return meanSatisfactionIndex;
    }
    
    public void SaveHackathon(double meanSatisfactionIndex)
    {
        Hackathon hackathon = new Hackathon
        {
            Employees = Employees.ToList()
        };
        HackathonRepository.AddHackathon(hackathon);
        
        hackathon.Employees = Employees.ToList();
        hackathon.Wishlists = Wishlists.ToList();
        hackathon.Teams = Teams;
        hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        
        ((List<Team>)hackathon.Teams!).ForEach(t =>
        {
            t.Hackathon = hackathon;
            t.HackathonId = hackathon.Id;
        });

        ((List<Employee>)hackathon.Employees!).
            ForEach(e => 
                ((List<Team>)e.Teams!).
                    AddRange( ((List<Team>)hackathon.Teams).
                    FindAll(t => (t.TeamLead.Id.Equals(e.Id) && t.TeamLead.Title.Equals(e.Title)) || (t.Junior.Id.Equals(e.Id) && t.Junior.Title.Equals(e.Title))
                    )));
        
        
        HackathonRepository.UpdateHackathon(hackathon);
    }
}
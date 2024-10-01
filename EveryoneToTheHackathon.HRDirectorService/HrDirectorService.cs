using System.Collections.Concurrent;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorService(IOptions<ServiceSettings> settings, HRDirector hrDirector, IHackathonRepository hackathonRepository)
{
    public readonly int EmployeesNumber = settings.Value.EmployeesNumber;
    public readonly int HackathonsNumber = settings.Value.HackathonsNumber;
    private HRDirector HrDirector { get; } = hrDirector;
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;

    public ConcurrentBag<Employee> Employees { get; set; } = [];
    public ConcurrentBag<Wishlist> Wishlists { get; set; } = [];
    public ConcurrentBag<Team> Teams { get; set; } = [];

    public TaskCompletionSource<bool> EmployeesGotTcs = new();
    public TaskCompletionSource<bool> WishlistsGotTcs = new();
    public TaskCompletionSource<bool> TeamsGotTcs = new();

    public async Task ResetAll()
    {
        await Task.Delay(1500);
        EmployeesGotTcs = new TaskCompletionSource<bool>();
        WishlistsGotTcs = new TaskCompletionSource<bool>();
        TeamsGotTcs = new TaskCompletionSource<bool>();
        Employees.Clear();
        Wishlists.Clear();
        Teams.Clear();
    }
    
    public double CalculationMeanSatisfactionIndex()
    {
        var teamleadsWishlists = Wishlists.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)).ToList();
        var juniorsWishlists = Wishlists.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)).ToList();
        var teams = Teams.ToList();
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(teamleadsWishlists, juniorsWishlists, teams);
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
        hackathon.Teams = Teams.ToList();
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
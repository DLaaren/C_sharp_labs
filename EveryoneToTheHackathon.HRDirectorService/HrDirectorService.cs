using System.Diagnostics;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;
using InvalidOperationException = System.InvalidOperationException;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorService(
    IOptions<ServiceSettings> settings, 
    HRDirector hrDirector, 
    IEmployeeRepository employeeRepository,
    IWishlistRepository wishlistRepository,
    ITeamRepository teamRepository,
    IHackathonRepository hackathonRepository)
{
    public readonly int EmployeesNumber = settings.Value.EmployeesNumber;
    public readonly int HackathonsNumber = settings.Value.HackathonsNumber;
    private HRDirector HrDirector { get; } = hrDirector;
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;
    private IEmployeeRepository EmployeeRepository { get; } = employeeRepository;
    private IWishlistRepository WishlistRepository { get; } = wishlistRepository;
    private ITeamRepository TeamRepository { get; } = teamRepository;
    
    public int CurrHackathonId { get; set; } = -1;
    public TaskCompletionSource<bool>? HackathonFinished;
    
    public int StartHackathon()
    {
        var hackathon = new Hackathon();
        HackathonRepository.AddHackathon(hackathon);
        return hackathon.Id;
    }
    
    public double CalculationMeanSatisfactionIndex(int hackathonId)
    {
        var wishlists = (List<Wishlist>) WishlistRepository.GetWishlistByHackathonId(hackathonId);
        var teamLeadsWishlists = wishlists.Where(w => w.EmployeeTitle == EmployeeTitle.TeamLead).ToList();
        var juniorsWishlists = wishlists.Where(w => w.EmployeeTitle == EmployeeTitle.Junior).ToList();
        var teams = (List<Team>) TeamRepository.GetTeamsByHackathonId(hackathonId);
        
        Debug.Assert(wishlists != null);
        Debug.Assert(teamLeadsWishlists != null);
        Debug.Assert(juniorsWishlists != null);
        Debug.Assert(teams != null);
        
        teams.ForEach(t =>
        {
            t.Employees = EmployeeRepository.GetEmployeesByTeamId(t.Id);
            t.TeamLead = ((List<Employee>)t.Employees).Find(e => e.Title == EmployeeTitle.TeamLead) ?? throw new InvalidOperationException();
            t.Junior = ((List<Employee>)t.Employees).Find(e => e.Title == EmployeeTitle.Junior) ?? throw new InvalidOperationException();
        });
        
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(teamLeadsWishlists, juniorsWishlists, teams);

        var hackathon = hackathonRepository.GetHackathonById(hackathonId);
        Debug.Assert(hackathon != null);
        hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        hackathonRepository.SaveHackathon(hackathon);
        
        return meanSatisfactionIndex;
    }
}
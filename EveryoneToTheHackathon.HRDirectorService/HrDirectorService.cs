using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;

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
        
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(teamLeadsWishlists, juniorsWishlists, teams);
        
        return meanSatisfactionIndex;
    }
}
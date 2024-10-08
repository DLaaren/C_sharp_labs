using System.Diagnostics;
using AutoMapper.Internal;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerService(
    IOptions<ControllerSettings> settings, 
    HRManager hrManager, 
    IHackathonRepository hackathonRepository, 
    IEmployeeRepository employeeRepository, 
    IWishlistRepository wishlistRepository,
    ITeamRepository teamRepository)
{
    public readonly int EmployeesNumber = settings.Value.EmployeesNumber;
    private HRManager HrManager { get; } = hrManager;
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;
    private IEmployeeRepository EmployeeRepository { get; } = employeeRepository;
    private IWishlistRepository WishlistRepository { get; } = wishlistRepository;
    private ITeamRepository TeamRepository { get; } = teamRepository;

    public void BuildTeamsAndSave(int hackathonId)
    {
        var hackathon = HackathonRepository.GetHackathonById(hackathonId);
        Debug.Assert(hackathon != null);

        var employees = (List<Employee>) EmployeeRepository.GetEmployeeByHackathonId(hackathonId);
        var wishlists = (List<Wishlist>) WishlistRepository.GetWishlistByHackathonId(hackathonId);
        var teamLeads = employees.Where(e => e.Title == EmployeeTitle.TeamLead).ToList();
        var juniors = employees.Where(e => e.Title == EmployeeTitle.Junior).ToList();
        var teamLeadsWishlists = wishlists.Where(w => w.EmployeeTitle == EmployeeTitle.TeamLead).ToList();
        var juniorsWishlists = wishlists.Where(w => w.EmployeeTitle == EmployeeTitle.Junior).ToList();
        
        var teams = (List<Team>)HrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
        teams.ForAll(t =>
        {
            t.Hackathon = hackathon;
            t.HackathonId = hackathonId;
        });
        
        TeamRepository.AddTeams(teams);
    }
}
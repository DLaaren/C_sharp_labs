using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Services;

public class HRDirectorService : BackgroundService
{
    private readonly ILogger<HRDirectorService> _logger;
    private readonly HttpClient _httpClient;
    private readonly HRDirector _hrDirector;
    public int EmployeesNumber { get; init; }
    public List<Employee>? Employees { get; set; }
    public List<Wishlist>? Wishlists { get; set; }
    public List<Team>? Teams { get; set; }
    private HackathonRepository _hackathonRepository;

    public HRDirectorService(ILogger<HRDirectorService> logger, HttpClient httpClient, HRDirector hrDirector, int employeesNumber, HackathonRepository hackathonRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hrDirector = hrDirector;
        EmployeesNumber = employeesNumber;
        _hackathonRepository = hackathonRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HRDirector waiting for employees, wishlists, teams;");

        if (Employees == null || Wishlists == null || Teams == null)
            await Task.Delay(1000, stoppingToken);

        double meanSatisfactionIndex = _hrDirector.CalculateMeanSatisfactionIndex(
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)),
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)),
            Teams!);
        
        _logger.LogInformation("HRDirector has counted mean satisfaction index");
        
        Hackathon hackathon = new Hackathon();
        _hackathonRepository.AddHackathon(hackathon);
        
        hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        hackathon.Employees = Employees;
        hackathon.Wishlists = Wishlists;
        hackathon.Teams = Teams;
        Teams!.ForEach(t =>
        {
            t.Hackathon = hackathon;
            t.HackathonId = hackathon.Id;
        });
        Employees!.ForEach(e => ((List<Team>)e.Teams!).AddRange( Teams.FindAll(t => t.TeamLead.Equals(e) || t.Junior.Equals(e))) );
        _hackathonRepository.UpdateHackathon(hackathon);
        
        _logger.LogInformation("HRDirector has stored all information to database;");
        
        await Task.CompletedTask;
    }
}
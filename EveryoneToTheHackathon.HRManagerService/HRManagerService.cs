using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.HRManagerService;

public class HRManagerService : BackgroundService
{
    private readonly ILogger<HRManagerService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _hrDirectorUrl;
    private readonly HRManager _hrManager;
    public int EmployeesNumber { get; init; }
    public IEnumerable<Employee>? Employees { get; set; }
    public IEnumerable<Wishlist>? Wishlists { get; set; }

    public HRManagerService(ILogger<HRManagerService> logger, HttpClient httpClient, string hrDirectorUrl, HRManager hrManager, int employeesNumber)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hrDirectorUrl = hrDirectorUrl;
        _hrManager = hrManager;
        EmployeesNumber = employeesNumber;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HRManager waiting for employees;");

        if (Employees == null)
            await Task.Delay(1000, stoppingToken);
            
        await SendEmployeesAsync(Employees!, stoppingToken);
        _logger.LogInformation("HRManager has sent employees;");
        
        
        _logger.LogInformation("HRManager waiting for wishlists;");
        if (Wishlists == null)
            await Task.Delay(1000, stoppingToken);
        
        await SendWishlistsAsync(Wishlists!, stoppingToken);
        _logger.LogInformation("HRManager has sent wishlists;");

        
        IEnumerable<Team> teams = _hrManager.BuildTeams(
            Employees!.Where(e => e.Title.Equals(EmployeeTitle.TeamLead)), 
            Employees!.Where(e => e.Title.Equals(EmployeeTitle.Junior)),
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)),
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)));
        _logger.LogInformation("HRManager has created teams;");

        await SendTeamsAsync(teams, stoppingToken);
        _logger.LogInformation("HRManager has sent teams;");

        await Task.CompletedTask;
    }
    
    private async Task SendEmployeesAsync(IEnumerable<Employee> employees, CancellationToken stoppingToken)
    {
        var employeeDtos = employees.Select(e => new EmployeeDto(e.Id, e.Title, e.Name));
        
        var content = new StringContent(JsonSerializer.Serialize(employeeDtos), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrDirectorUrl, content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
    
    private async Task SendWishlistsAsync(IEnumerable<Wishlist> wishlists, CancellationToken stoppingToken)
    {
        var wishlistDtos = wishlists.Select(w => new WishlistDto(w.EmployeeId, w.EmployeeTitle, w.DesiredEmployees));
        
        var content = new StringContent(JsonSerializer.Serialize(wishlistDtos), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrDirectorUrl, content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
    
    private async Task SendTeamsAsync(IEnumerable<Team> teams, CancellationToken stoppingToken)
    {
        var teamDtos = teams.Select(t => new TeamDto(
            new EmployeeDto(t.TeamLead.Id, t.TeamLead.Title, t.TeamLead.Name), 
            new EmployeeDto(t.Junior.Id, t.Junior.Title, t.Junior.Name)));
        
        var content = new StringContent(JsonSerializer.Serialize(teamDtos), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrDirectorUrl, content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
}
using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Services;

public class EmployeeService : BackgroundService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _hrManagerUrl;
    private readonly Employee _employee;
    private readonly IEnumerable<Employee> _probableTeammates;

    public EmployeeService(ILogger<EmployeeService> logger, HttpClient httpClient, string hrManagerUrl, Employee employee, IEnumerable<Employee> probableTeammates)
    {
        _logger = logger;
        _httpClient = httpClient;
        _hrManagerUrl = hrManagerUrl;
        _employee = employee;
        _probableTeammates = probableTeammates;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmployeeAsync(_employee, stoppingToken);
        _logger.LogInformation("Employee {0} {1} has sent his data;", _employee.Name, _employee.Title);
        
        _logger.LogInformation("Employee {0} {1} making his wishlist;", _employee.Name, _employee.Title);
        Wishlist wishlist = _employee.MakeWishlist(_probableTeammates);
        await SendWishlistAsync(wishlist, stoppingToken);
        _logger.LogInformation("Employee {0} {1} has sent his wishlist;", _employee.Name, _employee.Title);
        
        await Task.CompletedTask;
    }

    private async Task SendEmployeeAsync(Employee employee, CancellationToken stoppingToken)
    {
        EmployeeDto employeeDto = new EmployeeDto(_employee.Id, _employee.Title, _employee.Name);
        var content = new StringContent(JsonSerializer.Serialize(employeeDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrManagerUrl, content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task SendWishlistAsync(Wishlist wishlist, CancellationToken stoppingToken)
    {
        WishlistDto wishlistDto = new WishlistDto(wishlist.EmployeeId, wishlist.EmployeeTitle, wishlist.DesiredEmployees);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrManagerUrl, content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
}
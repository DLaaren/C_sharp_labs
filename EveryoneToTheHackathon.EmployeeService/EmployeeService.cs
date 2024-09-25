using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeService : BackgroundService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly HttpClient _httpClient;
    private readonly Uri _hrManagerUrl;
    private readonly Employee _employee;
    private readonly IEnumerable<Employee> _probableTeammates;

    public EmployeeService(ILogger<EmployeeService> logger, HttpClient httpClient, Uri hrManagerUrl, Employee employee, IEnumerable<Employee> probableTeammates)
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
        _logger.LogInformation("Employee 'Id = {0} Title = {1} Name = {2}' has sent his data;", _employee.Id, _employee.Title, _employee.Name);
        
        _logger.LogInformation("Employee 'Id = {0} Title = {1} Name = {2}' making his wishlist;", _employee.Id, _employee.Title, _employee.Name);
        Wishlist wishlist = _employee.MakeWishlist(_probableTeammates);
        await SendWishlistAsync(wishlist, stoppingToken);
        _logger.LogInformation("Employee 'Id = {0} Title = {1} Name = {2}' has sent his wishlist;", _employee.Id, _employee.Title, _employee.Name);
        
        await Task.CompletedTask;
    }

    private async Task SendEmployeeAsync(Employee employee, CancellationToken stoppingToken)
    {
        EmployeeDto employeeDto = new EmployeeDto(employee.Id, employee.Title, employee.Name);
        var content = new StringContent(JsonSerializer.Serialize(employeeDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrManagerUrl + "api/hr_manager/employee", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task SendWishlistAsync(Wishlist wishlist, CancellationToken stoppingToken)
    {
        WishlistDto wishlistDto = new WishlistDto(wishlist.EmployeeId, wishlist.EmployeeTitle, wishlist.DesiredEmployees);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_hrManagerUrl + "api/hr_manager/wishlist", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
}
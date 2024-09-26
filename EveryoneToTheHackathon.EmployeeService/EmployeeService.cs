using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeService(
    ILogger<EmployeeService> logger,
    HttpClient httpClient,
    Employee employee,
    IEnumerable<Employee> probableTeammates)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmployeeAsync(employee, stoppingToken);
        logger.LogInformation("Employee 'Id = {Id} Title = {Title} Name = {Name}' has sent his data;", employee.Id, employee.Title, employee.Name);
        
        logger.LogInformation("Employee 'Id = {Id} Title = {Title} Name = {Name}' is making his wishlist;", employee.Id, employee.Title, employee.Name);
        var wishlist = employee.MakeWishlist(probableTeammates);
        await SendWishlistAsync(wishlist, stoppingToken);
        logger.LogInformation("Employee 'Id = {Id} Title = {Title} Name = {Name}' has sent his wishlist;", employee.Id, employee.Title, employee.Name);
        
        await Task.CompletedTask;
    }

    private async Task SendEmployeeAsync(Employee employeeToSend, CancellationToken stoppingToken)
    {
        var employeeDto = new EmployeeDto(employeeToSend.Id, employeeToSend.Title, employeeToSend.Name);
        var content = new StringContent(JsonSerializer.Serialize(employeeDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/employee", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(wishlistToSend.EmployeeId, wishlistToSend.EmployeeTitle, wishlistToSend.DesiredEmployees);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/wishlist", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
}
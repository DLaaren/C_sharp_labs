using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeBackgroundService(
    ILogger<EmployeeBackgroundService> logger,
    HttpClient httpClient,
    EmployeeService employeeService)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Waiting for a hackathon to start");
        while (true) {}
        // ReSharper disable once FunctionNeverReturns
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
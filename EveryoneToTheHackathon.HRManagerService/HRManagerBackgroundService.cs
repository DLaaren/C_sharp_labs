using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerBackgroundService(
    ILogger<HrManagerBackgroundService> logger,
    HttpClient httpClient,
    HrManagerService hrManagerService)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Waiting for a hackathon to start");
        while (true) { }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private async Task SendEmployeesAsync(IEnumerable<Employee> employees, CancellationToken stoppingToken)
    {
        var employeeDtos = employees.Select(e => new EmployeeDto(e.Id, e.Title, e.Name)).ToList();
        
        var content = new StringContent(JsonSerializer.Serialize(employeeDtos), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_director/employees", content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
    
    private async Task SendWishlistsAsync(IEnumerable<Wishlist> wishlists, CancellationToken stoppingToken)
    {
        var wishlistDtos = wishlists.Select(w => new WishlistDto(w.EmployeeId, w.EmployeeTitle, w.DesiredEmployees)).ToList();
        
        var content = new StringContent(JsonSerializer.Serialize(wishlistDtos), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_director/wishlists", content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
    
    private async Task SendTeamsAsync(IEnumerable<Team> teams, CancellationToken stoppingToken)
    {
        var teamDtos = teams.Select(t => new TeamDto(
            new EmployeeDto(t.TeamLead.Id, t.TeamLead.Title, t.TeamLead.Name), 
            new EmployeeDto(t.Junior.Id, t.Junior.Title, t.Junior.Name))).ToList();
        
        var content = new StringContent(JsonSerializer.Serialize(teamDtos), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_director/teams", content, stoppingToken);
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }

}
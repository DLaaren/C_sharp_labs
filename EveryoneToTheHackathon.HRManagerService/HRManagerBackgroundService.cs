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
    : BackgroundService, IConsumer<IHackathonStarted>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Waiting for a hackathon to start");
        await hrManagerService.HackathonStartedTcs.Task;
        
        logger.LogInformation("HRManager waiting for employees;");

        while (hrManagerService.Employees == null)
            await Task.Delay(1000, stoppingToken);

        await SendEmployeesAsync(hrManagerService.Employees!, stoppingToken);
        logger.LogInformation("HRManager has sent employees;");
        
        
        logger.LogInformation("HRManager waiting for wishlists;");
        while (hrManagerService.Wishlists == null)
            await Task.Delay(1000, stoppingToken);
        
        await SendWishlistsAsync(hrManagerService.Wishlists!, stoppingToken);
        logger.LogInformation("HRManager has sent wishlists;");

        
        var teams = hrManagerService.HrManager.BuildTeams(
            hrManagerService.Employees!.Where(e => e.Title.Equals(EmployeeTitle.TeamLead)).ToList(), 
            hrManagerService.Employees!.Where(e => e.Title.Equals(EmployeeTitle.Junior)).ToList(),
            hrManagerService.Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)).ToList(),
            hrManagerService.Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)).ToList());
        logger.LogInformation("HRManager has created teams;");

        await SendTeamsAsync(teams, stoppingToken);
        logger.LogInformation("HRManager has sent teams;");

        await Task.CompletedTask;
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
    
        public Task Consume(ConsumeContext<IHackathonStarted> context)
       {
           logger.LogInformation(context.Message.Message);
           hrManagerService.HackathonStartedTcs.SetResult(true);
           return Task.CompletedTask;
       }
}
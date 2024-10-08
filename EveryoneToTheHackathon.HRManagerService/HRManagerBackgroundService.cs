using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerBackgroundService(
    IBusControl busControl,
    ILogger<HrManagerBackgroundService> logger,
    HttpClient httpClient,
    HrManagerService hrManagerService)
    : BackgroundService, IConsumer<HackathonStarted>, IConsumer<EmployeeAndWishlistSent>
{
    private int ReadyEmployeesCount { get; set; }
    private int CurrHackathonId { get; set; } = -1;
    
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

    private void SendTeamsStoredAsyncViaMessage(int count)
    {
        busControl.Publish(
            new TeamsStored(count));
        logger.LogInformation("Teams has been stored");
    }
    
    public Task Consume(ConsumeContext<HackathonStarted> context)
    {
        CurrHackathonId = context.Message.HackathonId;
        logger.LogInformation("Hackathon with id = {hackathonId} has started", CurrHackathonId);
        
        return Task.CompletedTask;
    }
    
   public Task Consume(ConsumeContext<EmployeeAndWishlistSent> context)
   {
       while (CurrHackathonId == -1)
       {
           logger.LogWarning("Hackathon hasn't started yet");
           return Task.CompletedTask;
       }

       logger.LogInformation("Got message about stored Employee's with id = {id} title = {title} name = {name} data",
           context.Message.Id, context.Message.Title, context.Message.Name);

       ReadyEmployeesCount += 1;
       if (ReadyEmployeesCount < hrManagerService.EmployeesNumber) 
           return Task.CompletedTask;
       
       hrManagerService.BuildTeamsAndSave(CurrHackathonId);
       logger.LogInformation("Teams has been stored");
            
       SendTeamsStoredAsyncViaMessage(ReadyEmployeesCount);

       CurrHackathonId = -1;
       ReadyEmployeesCount = 0;
       logger.LogInformation("Waiting for a hackathon to start");
       return Task.CompletedTask;
   }
}
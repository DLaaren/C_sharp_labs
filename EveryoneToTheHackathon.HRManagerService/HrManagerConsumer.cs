using System.Diagnostics;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerConsumer(
    IBusControl busControl,
    ILogger<HrManagerBackgroundService> logger,
    HttpClient httpClient,
    HrManagerService hrManagerService) : IConsumer<HackathonStarted>, IConsumer<EmployeeAndWishlistSent>
{
    public async Task Consume(ConsumeContext<HackathonStarted> context)
    {
        hrManagerService.EmployeesAndWishlistsStored = new TaskCompletionSource<bool>();
        hrManagerService.CurrHackathonId = context.Message.HackathonId;
        logger.LogInformation("Hackathon with id = {hackathonId} has started", hrManagerService.CurrHackathonId);

        await hrManagerService.EmployeesAndWishlistsStored.Task;
        
        hrManagerService.BuildTeamsAndSave(hrManagerService.CurrHackathonId);
        logger.LogInformation("Teams has been stored");
            
        hrManagerService.SendTeamsStoredAsyncViaMessage(hrManagerService.ReadyEmployeesCount / 2);

        hrManagerService.CurrHackathonId = -1;
        hrManagerService.ReadyEmployeesCount = 0;
        logger.LogInformation("Waiting for a hackathon to start");
        
        await Task.CompletedTask;
    }
    
    public Task Consume(ConsumeContext<EmployeeAndWishlistSent> context)
    {
        logger.LogInformation("Got message about stored Employee's with id = {id} title = {title} name = {name} data",
            context.Message.Id, context.Message.Title, context.Message.Name);

        // hrManagerService.ReadyEmployeesCount += 1;
        // if (hrManagerService.ReadyEmployeesCount < hrManagerService.EmployeesNumber) return Task.CompletedTask;
        if (hrManagerService.CheckNumberOfReadyEmployees(hrManagerService.CurrHackathonId) < hrManagerService.EmployeesNumber) 
            return Task.CompletedTask;
       
        Debug.Assert(hrManagerService.EmployeesAndWishlistsStored != null);
        hrManagerService.EmployeesAndWishlistsStored.TrySetResult(true);

        return Task.CompletedTask;
    }
}
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeConsumer(
    ILogger<EmployeeBackgroundService> logger,
    EmployeeService employeeService) : IConsumer<HackathonStarted>
{
    public Task Consume(ConsumeContext<HackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        
        logger.LogInformation("Hackathon with id = {hackathonId} has started", hackathonId);
        
        var wishlist = employeeService.Employee.MakeWishlist(employeeService.ProbableTeammates);
        
        employeeService.SaveEmployeeAndWishlist(wishlist, hackathonId);
        logger.LogInformation("Employee has stored his data");
            
        employeeService.SendEmployeeAndWishlistStoredAsyncViaMessage();
        
        logger.LogInformation("Waiting for a hackathon to start");
        return Task.CompletedTask;
    }
}
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorBackgroundService(
    IBusControl busControl,
    ILogger<HrDirectorBackgroundService> logger,
    HrDirectorService hrDirectorService)
    : BackgroundService, IConsumer<EmployeeSent>, IConsumer<WishlistSent>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await AnnounceHackathon(stoppingToken);
        logger.LogInformation("HRDirector has announced start of the hackathon");
        
        logger.LogInformation("HRDirector waiting for employees, wishlists, teams;");

        await hrDirectorService.EmployeesGotTcs.Task;
        await hrDirectorService.WishlistsGotTcs.Task;
        await hrDirectorService.TeamsGotTcs.Task;
        
        // while (hrDirectorService.Employees == null || hrDirectorService.Wishlists == null || hrDirectorService.Teams == null)
        //     await Task.Delay(1000, stoppingToken);

        logger.LogInformation("HRDirector got all data;");
        
        var meanSatisfactionIndex = hrDirectorService.CalculationMeanSatisfactionIndex();

        hrDirectorService.SaveHackathon(meanSatisfactionIndex);
        logger.LogInformation("HRDirector has stored all information to database;");
        
        logger.LogInformation("HRDirector has counted mean satisfaction index: {index}", meanSatisfactionIndex);
        
        await Task.CompletedTask;
    }

    private async Task AnnounceHackathon(CancellationToken stoppingToken)
    {
        await busControl.Publish<HackathonStarted>(new HackathonStarted("Hackathon has started"), stoppingToken);
    }

    public Task Consume(ConsumeContext<EmployeeSent> context)
    {
        hrDirectorService.Employees.Add(new Employee(context.Message.Id, context.Message.Title, context.Message.Name));
        if (hrDirectorService.Employees.Count == hrDirectorService.EmployeesNumber)
            hrDirectorService.EmployeesGotTcs.SetResult(true);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<WishlistSent> context)
    {
        hrDirectorService.Wishlists.Add(new Wishlist(context.Message.EmployeeId, context.Message.EmployeeTitle, context.Message.DesiredEmployees));
        if (hrDirectorService.Wishlists.Count == hrDirectorService.EmployeesNumber)
            hrDirectorService.WishlistsGotTcs.SetResult(true);
        return Task.CompletedTask;
    }
}
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorBackgroundService(
    IBusControl busControl,
    ILogger<HrDirectorBackgroundService> logger,
    HrDirectorService hrDirectorService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await AnnounceHackathon(stoppingToken);
        logger.LogInformation("HRDirector has announced start of the hackathon");
        
        logger.LogInformation("HRDirector waiting for employees, wishlists, teams;");
        
        while (hrDirectorService.Employees == null || hrDirectorService.Wishlists == null || hrDirectorService.Teams == null)
            await Task.Delay(1000, stoppingToken);

        var meanSatisfactionIndex = hrDirectorService.CalculationMeanSatisfactionIndex();

        hrDirectorService.SaveHackathon(meanSatisfactionIndex);
        logger.LogInformation("HRDirector has stored all information to database;");
        
        logger.LogInformation("HRDirector has counted mean satisfaction index: {index}", meanSatisfactionIndex);
        
        await Task.CompletedTask;
    }

    private async Task AnnounceHackathon(CancellationToken stoppingToken)
    {
        await busControl.Publish<IHackathonStarted>(new
        {
            HackathonId = hrDirectorService.StartHackathon()
        }, stoppingToken);
    }
}
using System.Diagnostics;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorBackgroundService(
    IBusControl busControl,
    ILogger<HrDirectorBackgroundService> logger,
    HrDirectorService hrDirectorService)
    : BackgroundService, IConsumer<TeamsStored>
{
    private int CurrHackathonId { get; set; } = -1;
    private TaskCompletionSource<bool>? _hackathonFinished;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var i = 0; i < hrDirectorService.HackathonsNumber; i++)
        {
            await StartHackathon(stoppingToken);
            
            Debug.Assert(_hackathonFinished != null);
            await _hackathonFinished.Task;
        }
        await Task.CompletedTask;
    }

    private async Task StartHackathon(CancellationToken stoppingToken)
    {
        CurrHackathonId = hrDirectorService.StartHackathon();
        logger.LogInformation("Starting hackathon {id}", CurrHackathonId);
        
        _hackathonFinished = new TaskCompletionSource<bool>();
        
        await busControl.Publish(new HackathonStarted(CurrHackathonId), stoppingToken);
        logger.LogInformation("HRDirector has announced start of the hackathon");
    }
    

    public Task Consume(ConsumeContext<TeamsStored> context)
    {
        logger.LogInformation("HRManager has built {count} teams", context.Message.Count);
        
        var meanSatisfactionIndex = hrDirectorService.CalculationMeanSatisfactionIndex(CurrHackathonId);
        logger.LogInformation("HRDirector has counted mean satisfaction index: {index}", meanSatisfactionIndex);
        
        Debug.Assert(_hackathonFinished != null);
        _hackathonFinished.TrySetResult(true);
        return Task.CompletedTask;
    }
}
using System.Diagnostics;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorConsumer(
    ILogger<HrDirectorBackgroundService> logger,
    HrDirectorService hrDirectorService) : IConsumer<TeamsStored>
{
    public Task Consume(ConsumeContext<TeamsStored> context)
    {
        logger.LogInformation("HRManager has built {count} teams", context.Message.Count);
        
        var meanSatisfactionIndex = hrDirectorService.CalculationMeanSatisfactionIndex(hrDirectorService.CurrHackathonId);
        logger.LogInformation("HRDirector has counted mean satisfaction index: {index}", meanSatisfactionIndex);
        
        Debug.Assert(hrDirectorService.HackathonFinished != null);
        hrDirectorService.HackathonFinished.TrySetResult(true);
        return Task.CompletedTask;
    }
}
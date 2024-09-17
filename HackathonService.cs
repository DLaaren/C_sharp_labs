using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace lab1;

public class HackathonService : IHostedService
{
    private Hackathon _hackathon;
    private readonly ILogger _logger;

    public HackathonService(ILogger<HackathonService> logger, Hackathon hackathon)
    {
        _logger = logger;
        _hackathon = hackathon;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HackathonService");
        
        MainTask(cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping HackathonService");
        return Task.CompletedTask;
    }

    private void MainTask(CancellationToken cancellationToken, int hackathonNumber = 1000)
    {
        double meanSatisfactionIndexForAllRounds = 0;
        for (int i = 1; i <= hackathonNumber; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _hackathon.HoldEvent();
            Console.WriteLine("Mean satisfaction index for {0}th round = {1}",
                i.ToString(), _hackathon.MeanSatisfactionIndex);
            meanSatisfactionIndexForAllRounds += _hackathon.MeanSatisfactionIndex;
        }

        Console.WriteLine(
            "Mean satisfaction index for all rounds = " + meanSatisfactionIndexForAllRounds / hackathonNumber);
    }
}
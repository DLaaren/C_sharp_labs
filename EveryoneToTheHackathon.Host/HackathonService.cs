using EveryoneToTheHackathon.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

public class HackathonService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IHackathon _hackathon;
    private readonly int _hackathonNumber;
    
    public HackathonService(ILogger<HackathonService> logger, IHackathon hackathon, int hackathonNumber)
    {
        _logger = logger;
        _hackathon = hackathon;
        _hackathonNumber = hackathonNumber;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HackathonService");
        _logger.LogInformation("[HackathonService] : parsing csv files");

        _logger.LogInformation("[HackathonService] : performing main work");
        MainTask(cancellationToken, _hackathonNumber);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping HackathonService");
        return Task.CompletedTask;
    }

    private void MainTask(CancellationToken cancellationToken, int hackathonNumber = 1000)
    {
        Hackathon hackathon = (Hackathon)_hackathon;
        double meanSatisfactionIndexForAllRounds = 0;
        for (int i = 1; i <= hackathonNumber; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _hackathon.HoldEvent();
            Console.WriteLine("Mean satisfaction index for {0}th round = {1}",
                i.ToString(), hackathon.MeanSatisfactionIndex);
            meanSatisfactionIndexForAllRounds += hackathon.MeanSatisfactionIndex;
        }

        Console.WriteLine(
            "Mean satisfaction index for all rounds = " + meanSatisfactionIndexForAllRounds / hackathonNumber);
    }
}
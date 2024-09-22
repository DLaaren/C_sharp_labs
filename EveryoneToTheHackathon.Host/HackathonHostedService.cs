using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

public class HackathonHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HackathonHostedService> _logger;
    private IHackathon? _hackathon;
    private readonly int _hackathonRounds;
    
    private readonly IHackathonService _hackathonService;
    private readonly IEmployeeService _employeeService;
    private readonly IWishlistService _wishlistService;
    private readonly ITeamService _teamService;
    
    public HackathonHostedService(IServiceProvider serviceProvider, ILogger<HackathonHostedService> logger, int hackathonRounds,
        IHackathonService hackathonService, IEmployeeService employeeService, IWishlistService wishlistService, ITeamService teamService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hackathonRounds = hackathonRounds;
        _hackathonService = hackathonService;
        _employeeService = employeeService;
        _wishlistService = wishlistService;
        _teamService = teamService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HackathonHostedService");
        
        for (int round = 1; round <= _hackathonRounds; round++)
        {
            _hackathon = _serviceProvider.GetRequiredService<IHackathon>();
            StartHackathon(_hackathon, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void StartHackathon(IHackathon hackathon, CancellationToken cancellationToken)
    {
        _hackathonService.AddHackathon(hackathon);

        cancellationToken.ThrowIfCancellationRequested();
        hackathon.HoldEvent();
        _hackathonService.UpdateHackathon(hackathon);
        _logger.LogInformation(
           "Mean satisfaction index for all rounds = " + _hackathonService.GetMeanSatisfactionIndexForAllRounds());
    }
}
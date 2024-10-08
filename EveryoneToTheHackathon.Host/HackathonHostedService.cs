using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

public class HackathonHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HackathonHostedService> _logger;
    private Hackathon? _hackathon;
    private readonly int _hackathonRounds;
    
    private readonly IHackathonRepository _hackathonRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ITeamRepository _teamRepository;
    
    public HackathonHostedService(IServiceProvider serviceProvider, ILogger<HackathonHostedService> logger, int hackathonRounds,
        IHackathonRepository hackathonRepository, IEmployeeRepository employeeRepository, IWishlistRepository wishlistRepository, ITeamRepository teamRepository)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hackathonRounds = hackathonRounds;
        _hackathonRepository = hackathonRepository;
        _employeeRepository = employeeRepository;
        _wishlistRepository = wishlistRepository;
        _teamRepository = teamRepository;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HackathonHostedService");
        
        for (int round = 1; round <= _hackathonRounds; round++)
        {
            _hackathon = _serviceProvider.GetRequiredService<Hackathon>();
            StartHackathon(_hackathon, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void StartHackathon(Hackathon hackathon, CancellationToken cancellationToken)
    {
        _hackathonRepository.AddHackathon(hackathon);
        cancellationToken.ThrowIfCancellationRequested();
        hackathon.HoldEvent();
        _hackathonRepository.UpdateHackathon(hackathon);
        _logger.LogInformation(
           "Mean satisfaction index for all rounds = " + _hackathonRepository.GetMeanSatisfactionIndexForAllRounds());
    }
}
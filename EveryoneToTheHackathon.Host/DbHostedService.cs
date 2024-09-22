using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

public class DbHostedService : IHostedService
{
    private readonly ILogger<DbHostedService> _logger;
    private readonly AppDbContext _dbContext;

    public DbHostedService(ILogger<DbHostedService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting DbHostedService");
        await PreloadData(_dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task PreloadData(AppDbContext dbContext)
    {
        _logger.LogInformation("Preloading data");
        dbContext.Hackathons.RemoveRange(dbContext.Hackathons);
        dbContext.Employees.RemoveRange(dbContext.Employees);
        dbContext.Wishlists.RemoveRange(dbContext.Wishlists);
        dbContext.Teams.RemoveRange(dbContext.Teams);
        
        await dbContext.SaveChangesAsync();
    }
}
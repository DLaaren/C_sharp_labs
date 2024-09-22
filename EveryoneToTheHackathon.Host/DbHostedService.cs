using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

public class DbHostedService : IHostedService
{
    private readonly ILogger<DbHostedService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly EmployeesSeedData? _employeesSeedData;

    public DbHostedService(ILogger<DbHostedService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public DbHostedService(ILogger<DbHostedService> logger, AppDbContext dbContext, EmployeesSeedData employeesSeedData)
    {
        _logger = logger;
        _dbContext = dbContext;
        _employeesSeedData = employeesSeedData;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting DbHostedService");
        await (_employeesSeedData != null ? PreloadData(_dbContext) : Task.CompletedTask);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task PreloadData(AppDbContext dbContext)
    {
        _logger.LogInformation("Preloading data");
        dbContext.Employees.RemoveRange(dbContext.Employees);
        dbContext.Employees.AddRange(_employeesSeedData!.Employees);
        await dbContext.SaveChangesAsync();
    }
}
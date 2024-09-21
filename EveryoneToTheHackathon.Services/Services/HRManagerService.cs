using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;

namespace EveryoneToTheHackathon.Services.Services;

public class HRManagerService(AppDbContext dbContext) : IHRManagerService
{
    private readonly AppDbContext _dbContext = dbContext;
}
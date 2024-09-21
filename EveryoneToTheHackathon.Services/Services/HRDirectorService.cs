using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;

namespace EveryoneToTheHackathon.Services.Services;

public class HRDirectorService(AppDbContext dbContext) : IHRDirectorService
{
    private readonly AppDbContext _dbContext = dbContext;
}
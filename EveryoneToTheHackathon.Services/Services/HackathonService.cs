using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services.Services;

public class HackathonService(AppDbContext dbContext) : IHackathonService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Hackathon>> GetHackathonsAsync()
    {
        return await _dbContext.Hackathons.ToListAsync();
    }
    
    public async Task AddHackathonsAsync(IEnumerable<Hackathon> hackathons)
    {
        _dbContext.AddRange(hackathons);
        await _dbContext.SaveChangesAsync();
    } 
}
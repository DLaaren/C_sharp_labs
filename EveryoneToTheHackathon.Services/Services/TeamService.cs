using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services.Services;

public class TeamService(AppDbContext dbContext) : ITeamService
{
    private readonly AppDbContext _dbContext = dbContext;
    
    public async Task<IEnumerable<Team>> GetTeamsAsync()
    {
        return await _dbContext.Teams.ToListAsync();
    }
    
    public async Task AddTeamsAsync(IEnumerable<Team> teams)
    {
        _dbContext.AddRange(teams);
        await _dbContext.SaveChangesAsync();
    }
}
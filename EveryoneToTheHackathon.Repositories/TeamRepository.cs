using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class TeamRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : ITeamRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddTeams(IEnumerable<Team> teams)
    {
        _dbContext.AddRange(teams);
        _dbContext.SaveChanges();
    }

    public IEnumerable<Team> GetTeamsByHackathonId(int hackathonId)
    {
        return _dbContext.Teams.Where(t => t.Hackathon != null && t.Hackathon.Id == hackathonId).ToList();
    }
}
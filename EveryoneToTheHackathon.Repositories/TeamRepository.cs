using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class TeamRepository(AppDbContext dbContext) : ITeamRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Team? GetTeamById(int teamId)
    {
        return _dbContext.Teams.Find(teamId);
    }

    public IEnumerable<Team> GetTeams()
    {
        return _dbContext.Teams.ToList();
    }

    public void AddTeam(Team team)
    {
        _dbContext.Add(team);
        _dbContext.SaveChanges();
    }

    public void AddTeams(IEnumerable<Team> teams)
    {
        _dbContext.AddRange(teams);
        _dbContext.SaveChanges();
    }

    public void UpdateTeam(Team team)
    {
        _dbContext.Update(team);
        _dbContext.SaveChanges();
    }

    public void UpdateTeams(IEnumerable<Team> teams)
    {
        _dbContext.UpdateRange(teams);
        _dbContext.SaveChanges();
    }
}
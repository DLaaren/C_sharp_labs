using System.Diagnostics;
using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class TeamRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : ITeamRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddTeam(Team team)
    {
        _dbContext.Entry(team).State = EntityState.Added;
        var teamLead = ((List<Employee>)team.Employees).Find(e => e.Title == EmployeeTitle.TeamLead);
        var junior =  ((List<Employee>)team.Employees).Find(e => e.Title == EmployeeTitle.Junior);
        Debug.Assert(teamLead != null);
        Debug.Assert(junior != null);
        _dbContext.Entry(teamLead).State = EntityState.Modified;
        _dbContext.Entry(junior).State = EntityState.Modified;
    }

    public void UpdateTeam(Team team)
    {
        if (!_dbContext.Teams.Any(t => t.Id == team.Id))
            AddTeam(team);
        else
            _dbContext.Update(team);
    }
    
    public void SaveTeams(IEnumerable<Team> teams)
    {
        foreach (var team in (List<Team>)teams)
        {
            UpdateTeam(team);
            _dbContext.SaveChanges();
        }
    }

    public IEnumerable<Team> GetTeamsByHackathonId(int hackathonId)
    {
        return _dbContext.Teams.Where(t => t.Hackathon != null && t.Hackathon.Id == hackathonId).ToList();
    }
}
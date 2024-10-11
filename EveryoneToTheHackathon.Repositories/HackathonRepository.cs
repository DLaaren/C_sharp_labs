using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class HackathonRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : IHackathonRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public Hackathon? GetHackathonById(int hackathonId)
    {
        return _dbContext.Hackathons.Find(hackathonId);
    }

    public void AddHackathon(Hackathon hackathon)
    {
        var allIds = _dbContext.Employees.Select(e => e.Id).ToList();
        foreach (var employee in hackathon.Employees!)
        {
            if (!allIds.Contains(employee.Id))
                _dbContext.Employees.Add(employee);
        }

        _dbContext.Entry(hackathon).State = EntityState.Added;
        _dbContext.SaveChanges();
    }
    

    public void UpdateHackathon(Hackathon hackathon)
    {
        _dbContext.Update(hackathon);
        _dbContext.SaveChanges();
    }

    public void SaveHackathon(Hackathon hackathon)
    {
        _dbContext.Entry(hackathon).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    public double GetMeanSatisfactionIndexForAllRounds()
    {
        var hackathons = _dbContext.Hackathons.ToList();
        return hackathons.Select(h => h.MeanSatisfactionIndex).Sum() / hackathons.Count;
    }
}
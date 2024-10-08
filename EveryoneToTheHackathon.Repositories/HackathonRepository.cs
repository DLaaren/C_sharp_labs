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
    
    public IEnumerable<Hackathon> GetHackathons()
    {
        return _dbContext.Hackathons.ToList();
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

    public void AddHackathons(IEnumerable<Hackathon> hackathons)
    {
        _dbContext.AddRange(hackathons);
        _dbContext.SaveChanges();
    }

    public void UpdateHackathon(Hackathon hackathon)
    {
        //_dbContext.UpdateRange(((Hackathon)hackathon).Employees);
        _dbContext.Update(hackathon);
        _dbContext.SaveChanges();
    }

    public void UpdateHackathons(IEnumerable<Hackathon> hackathons)
    {
        ((List<Hackathon>)hackathons).ForEach(UpdateHackathon);
        _dbContext.SaveChanges();
    }

    public double GetMeanSatisfactionIndexForAllRounds()
    {
        var hackathons = _dbContext.Hackathons.ToList();
        return hackathons.Select(h => h.MeanSatisfactionIndex).Sum() / hackathons.Count;
    }

    public void DeleteHackathon(int hackathonId)
    {
        var hackathon = _dbContext.Hackathons.Find(hackathonId);
        if (hackathon == null) return;
        _dbContext.Remove(hackathon);
        _dbContext.SaveChanges();
    }
}
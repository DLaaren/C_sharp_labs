using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class HackathonRepository(/*AppDbContext dbContext,*/ IDbContextFactory<AppDbContext> myDbContextFactory) : IHackathonRepository
{
    private AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    //private readonly AppDbContext _dbContext = dbContext;

    public IHackathon? GetHackathonById(int hackathonId)
    {
        return _dbContext.Hackathons.Find(hackathonId);
    }
    
    public IEnumerable<IHackathon> GetHackathons()
    {
        return _dbContext.Hackathons.ToList();
    }

    public void AddHackathon(IHackathon hackathon)
    {
        if (((Hackathon)hackathon).Employees != null)
        {
            var allIds = _dbContext.Employees.Select(e => e.Id).ToList();
            foreach (var employee in ((Hackathon)hackathon).Employees!)
            {
                if (!allIds.Contains(employee.Id))
                    _dbContext.Employees.Add(employee);
            }
        }

        _dbContext.Entry(hackathon).State = EntityState.Added;
        _dbContext.SaveChanges();
    }

    public void AddHackathons(IEnumerable<IHackathon> hackathons)
    {
        _dbContext.AddRange(hackathons);
        _dbContext.SaveChanges();
    }

    public void UpdateHackathon(IHackathon hackathon)
    {
        _dbContext.UpdateRange(((Hackathon)hackathon).Employees);
        _dbContext.Update(hackathon);
        _dbContext.SaveChanges();
    }

    public void UpdateHackathons(IEnumerable<IHackathon> hackathons)
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
        Hackathon? hackathon = _dbContext.Hackathons.Find(hackathonId);
        if (hackathon != null)
        {
            _dbContext.Remove(hackathon);
            _dbContext.SaveChanges();
        }
    }
}
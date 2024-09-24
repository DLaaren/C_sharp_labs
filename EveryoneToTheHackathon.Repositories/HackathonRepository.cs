using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class HackathonRepository(AppDbContext dbContext) : IHackathonRepository
{
    private readonly AppDbContext _dbContext = dbContext;

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
        _dbContext.Add(hackathon);
        _dbContext.SaveChanges();
    }

    public void AddHackathons(IEnumerable<IHackathon> hackathons)
    {
        _dbContext.AddRange(hackathons);
        _dbContext.SaveChanges();
    }

    public void UpdateHackathon(IHackathon hackathon)
    {
        /*var existingHackathon = _dbContext.Hackathons
            .Include(h => h.Teams)
            .Include(h => h.Employees)
            .Include(h => h.Wishlists)
            .FirstOrDefault(h => h.Id == ((Hackathon)updatedHackathon).Id);
    
        if (existingHackathon != null)
        {
            existingHackathon.MeanSatisfactionIndex = ((Hackathon)updatedHackathon).MeanSatisfactionIndex;
    
            existingHackathon.Teams = ((Hackathon)updatedHackathon).Teams;
            existingHackathon.Employees = ((Hackathon)updatedHackathon).Employees;
            existingHackathon.Wishlists = ((Hackathon)updatedHackathon).Wishlists;
            foreach (var wishlist in existingHackathon.Wishlists)
            {
                var existingWishlist = _dbContext.Wishlists
                    .FirstOrDefault(w => w.EmployeeId == wishlist.EmployeeId);
                if (existingWishlist != null)
                {
                    existingWishlist.;
                }
            }

            _dbContext.SaveChanges();
        }*/
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
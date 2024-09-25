using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IHackathonRepository
{
    public IHackathon? GetHackathonById(int hackathonId);
    public IEnumerable<IHackathon> GetHackathons();
    public void AddHackathon(IHackathon hackathon);
    public void AddHackathons(IEnumerable<IHackathon> hackathons);
    public void UpdateHackathon(IHackathon hackathon);
    public void UpdateHackathons(IEnumerable<IHackathon> hackathons);
    public double GetMeanSatisfactionIndexForAllRounds();
    public void DeleteHackathon(int hackathonId);
}
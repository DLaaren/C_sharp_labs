using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IHackathonRepository
{
    public Hackathon? GetHackathonById(int hackathonId);
    public IEnumerable<Hackathon> GetHackathons();
    public void AddHackathon(Hackathon hackathon);
    public void AddHackathons(IEnumerable<Hackathon> hackathons);
    public void UpdateHackathon(Hackathon hackathon);
    public void UpdateHackathons(IEnumerable<Hackathon> hackathons);
    public double GetMeanSatisfactionIndexForAllRounds();
    public void DeleteHackathon(int hackathonId);
}
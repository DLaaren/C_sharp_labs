using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IHackathonRepository
{
    public Hackathon? GetHackathonById(int hackathonId);
    public void AddHackathon(Hackathon hackathon);
    public void UpdateHackathon(Hackathon hackathon);
    public void SaveHackathon(Hackathon hackathon);
    public double GetMeanSatisfactionIndexForAllRounds();
}
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface ITeamRepository
{
    public void AddTeams(IEnumerable<Team> teams);
    public IEnumerable<Team> GetTeamsByHackathonId(int hackathonId);
}
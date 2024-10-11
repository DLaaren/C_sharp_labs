using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface ITeamRepository
{
    public void AddTeam(Team team);
    public void UpdateTeam(Team team);
    public void SaveTeams(IEnumerable<Team> teams);
    public IEnumerable<Team> GetTeamsByHackathonId(int hackathonId);
}
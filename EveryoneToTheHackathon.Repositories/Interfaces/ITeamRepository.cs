using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface ITeamRepository
{
    public Team? GetTeamById(int teamId);
    public IEnumerable<Team> GetTeams();
    public void AddTeam(Team team);
    public void AddTeams(IEnumerable<Team> teams);
    public void UpdateTeam(Team team);
    public void UpdateTeams(IEnumerable<Team> teams);
}
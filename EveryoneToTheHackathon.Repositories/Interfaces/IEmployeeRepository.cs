using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IEmployeeRepository
{
    public void AddEmployee(Employee employee);
    public void UpdateEmployee(Employee employee);
    public void SaveEmployee(Employee employee, Hackathon hackathon);
    public IEnumerable<Employee> GetEmployeeByHackathonId(int hackathonId);
    public IEnumerable<Employee> GetEmployeesByTeamId(int teamId);
}
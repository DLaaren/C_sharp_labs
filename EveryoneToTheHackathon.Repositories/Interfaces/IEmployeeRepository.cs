using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IEmployeeRepository
{
    public void AddEmployee(Employee employee);
    public IEnumerable<Employee> GetEmployeeByHackathonId(int hackathonId);
}
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Host;

public class EmployeesSeedData
{
    public IEnumerable<Employee> Employees { get; init; }

    public EmployeesSeedData(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors)
    {
        Employees = teamLeads.Concat(juniors).ToList();
    }
}
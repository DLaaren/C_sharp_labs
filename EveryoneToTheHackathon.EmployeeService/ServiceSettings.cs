using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.EmployeeService;

public class ServiceSettings
{
    public Employee Employee { get; set; }
    public IEnumerable<Employee> ProbableTeammates { get; set; }
}
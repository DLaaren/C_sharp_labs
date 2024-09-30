using EveryoneToTheHackathon.Entities;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeService(IOptions<ServiceSettings> settings)
{
    public Employee Employee { get; } = settings.Value.Employee;
    public IEnumerable<Employee> ProbableTeammates { get; } = settings.Value.ProbableTeammates;
    public readonly TaskCompletionSource<bool> HackathonStartedTcs = new();
} 
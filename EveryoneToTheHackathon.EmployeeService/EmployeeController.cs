using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneToTheHackathon.EmployeeService;

[ApiController]
[Route("api/employee")]
public class EmployeeController(EmployeeService employeeService)
: ControllerBase
{
    private EmployeeService EmployeeService { get; } = employeeService;
    
    [HttpGet("health"), AllowAnonymous]
    public Task<IActionResult> HealthCheck() => Task.FromResult<IActionResult>(Ok());
}

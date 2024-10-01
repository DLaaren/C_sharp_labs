using System.Collections.Concurrent;
using System.Diagnostics;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneToTheHackathon.HRDirectorService;

[ApiController]
[Route("api/hr_director")]
public class HrDirectorController(HrDirectorService hrDirectorService)
    : ControllerBase
{
    private HrDirectorService HrDirectorService { get; } = hrDirectorService;

    [HttpPost("employees"), AllowAnonymous]
    public Task<IActionResult> GetEmployees([FromBody] List<EmployeeDto> employeeDtos)
    {
        var employees = employeeDtos.
            Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name)).ToList();
        
        Debug.Assert(employees.Count == HrDirectorService.EmployeesNumber);

        HrDirectorService.Employees = new ConcurrentBag<Employee>(employees);
        
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpPost("wishlists"), AllowAnonymous]
    public Task<IActionResult> GetWishlists([FromBody] List<WishlistDto> wishlistDtos)
    {
        var wishlists = wishlistDtos.
            Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees)).ToList();

        Debug.Assert(wishlists.Count == HrDirectorService.EmployeesNumber);
        
        HrDirectorService.Wishlists = new ConcurrentBag<Wishlist>(wishlists);
        
        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpPost("teams"), AllowAnonymous]
    public Task<IActionResult> GetTeams([FromBody] List<TeamDto> teamDtos)
    {
        var teams = teamDtos.
            Select(tDto => new Team(
                new Employee(tDto.TeamLead.Id, tDto.TeamLead.Title, tDto.TeamLead.Name), 
                new Employee(tDto.Junior.Id, tDto.Junior.Title, tDto.Junior.Name))).ToList();

        Debug.Assert(teams.Count == HrDirectorService.EmployeesNumber / 2);
        
        HrDirectorService.Teams = teams;
        HrDirectorService.TeamsGotTcs.SetResult(true);
        
        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpGet("health"), AllowAnonymous]
    public Task<IActionResult> HealthCheck() => Task.FromResult<IActionResult>(Ok());
}

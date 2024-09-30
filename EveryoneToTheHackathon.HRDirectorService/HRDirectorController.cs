using System.Diagnostics;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRDirectorService;

[ApiController]
[Route("api/hr_director")]
public class HrDirectorController(IOptions<ControllerSettings> settingsOptions, HrDirectorService hrDirectorService)
    : ControllerBase
{
    private HrDirectorService HrDirectorService { get; } = hrDirectorService;
    private readonly ControllerSettings _settings = settingsOptions.Value;

    [HttpPost("employees"), AllowAnonymous]
    public Task<IActionResult> GetEmployees([FromBody] List<EmployeeDto> employeeDtos)
    {
        var employees = employeeDtos.
            Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name)).ToList();
        
        Debug.Assert(employees.Count == _settings.EmployeesNumber);

        HrDirectorService.Employees = employees;
        
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpPost("wishlists"), AllowAnonymous]
    public Task<IActionResult> GetWishlists([FromBody] List<WishlistDto> wishlistDtos)
    {
        var wishlists = wishlistDtos.
            Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees)).ToList();

        Debug.Assert(wishlists.Count == _settings.EmployeesNumber);
        
        HrDirectorService.Wishlists = wishlists;
        
        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpPost("teams"), AllowAnonymous]
    public Task<IActionResult> GetTeams([FromBody] List<TeamDto> teamDtos)
    {
        var teams = teamDtos.
            Select(tDto => new Team(
                new Employee(tDto.TeamLead.Id, tDto.TeamLead.Title, tDto.TeamLead.Name), 
                new Employee(tDto.Junior.Id, tDto.Junior.Title, tDto.Junior.Name))).ToList();

        Debug.Assert(teams.Count == _settings.EmployeesNumber / 2);
        
        HrDirectorService.Teams = teams;
        
        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpGet("health"), AllowAnonymous]
    public Task<IActionResult> HealthCheck() => Task.FromResult<IActionResult>(Ok());
}

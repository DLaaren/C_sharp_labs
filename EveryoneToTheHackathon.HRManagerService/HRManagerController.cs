using System.Collections.Concurrent;
using System.Diagnostics;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRManagerService;

[ApiController]
[Route("api/hr_manager")]
public class HrManagerController(IOptions<ControllerSettings> settingsOptions, HrManagerService hrManagerService)
    : ControllerBase
{
    private static readonly ConcurrentBag<EmployeeDto> EmployeeDtos = [];
    private static readonly ConcurrentBag<WishlistDto> WishlistDtos = [];

    private HrManagerService HrManagerService { get; } = hrManagerService;
    private readonly ControllerSettings _settings = settingsOptions.Value;

    [HttpPost("employee"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public Task<IActionResult> GetEmployees([FromBody] EmployeeDto employeeDto)
    {
        EmployeeDtos.Add(employeeDto);

        if (EmployeeDtos.Count < _settings.EmployeesNumber) return Task.FromResult<IActionResult>(Ok());
        
        var employees = new List<EmployeeDto>(EmployeeDtos).
            Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name)).ToList();
            
        EmployeeDtos.Clear();

        HrManagerService.Employees = employees;
        Debug.Assert(HrManagerService.Employees!.Count == _settings.EmployeesNumber);

        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpPost("wishlist"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public Task<IActionResult> GetWishlists([FromBody] WishlistDto wishlistDto)
    {
        WishlistDtos.Add(wishlistDto);

        if (WishlistDtos.Count < _settings.EmployeesNumber) return Task.FromResult<IActionResult>(Ok());
        
        var wishlists = new List<WishlistDto>(WishlistDtos).
            Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees)).ToList();
            
        WishlistDtos.Clear();

        HrManagerService.Wishlists = wishlists;
        Debug.Assert(HrManagerService.Wishlists!.Count == _settings.EmployeesNumber);

        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpGet("health"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public Task<IActionResult> HealthCheck() => Task.FromResult<IActionResult>(Ok());
}
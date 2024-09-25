using System.Collections.Concurrent;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRManagerService;

[ApiController]
[Route("api/hr_manager")]
public class HRManagerController : ControllerBase
{
    private static readonly ConcurrentBag<EmployeeDto> _employeetDtos = new();
    private static readonly ConcurrentBag<WishlistDto> _wishlistDtos = new();
    
    private readonly HRManagerService _hrManagerService;
    private readonly ControllerSettings _settings;

    public HRManagerController(HRManagerService hrManagerService, IOptions<ControllerSettings> settingsOptions)
    {
        _hrManagerService = hrManagerService;
        _settings = settingsOptions.Value;
    }

    [HttpPost("employee"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public Task<IActionResult> GetEmployees([FromBody] EmployeeDto employeeDto)
    {
        _employeetDtos.Add(employeeDto);

        if (_employeetDtos.Count >= _settings.EmployeesNumber)
        {
            var employees = new List<EmployeeDto>(_employeetDtos).
                Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name));
            
            _employeetDtos.Clear();

            _hrManagerService.Employees = employees;
        }

        return Task.FromResult<IActionResult>(Ok());
    }
    
    [HttpPost("wishlist"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public Task<IActionResult> GetWishlists([FromBody] WishlistDto wishlistDto)
    {
        _wishlistDtos.Add(wishlistDto);

        if (_wishlistDtos.Count >= _settings.EmployeesNumber)
        {
            var wishlists = new List<WishlistDto>(_wishlistDtos).
                Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees));
            
            _wishlistDtos.Clear();

            _hrManagerService.Wishlists = wishlists;
        }

        return Task.FromResult<IActionResult>(Ok());
    }
}
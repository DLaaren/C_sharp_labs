using System.Collections.Concurrent;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Services;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneToTheHackathon.Controllers;

[ApiController]
[Route("api/hr_manager")]
public class HRManagerController : ControllerBase
{
    private readonly HRManagerService _hrManagerService;
    private static readonly ConcurrentBag<EmployeeDto> _employeetDtos = new();
    private static readonly ConcurrentBag<WishlistDto> _wishlistDtos = new();

    public HRManagerController(HRManagerService hrManagerService)
    {
        _hrManagerService = hrManagerService;
    }

    [HttpPost("employee")]
    public void GetEmployees([FromBody] EmployeeDto employeeDto)
    {
        _employeetDtos.Add(employeeDto);

        if (_employeetDtos.Count >= _hrManagerService.EmployeesNumber)
        {
            var employees = new List<EmployeeDto>(_employeetDtos).
                Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name));
            
            _employeetDtos.Clear();

            _hrManagerService.Employees = employees;
        }
    }
    
    [HttpPost("wishlist")]
    public void GetWishlists([FromBody] WishlistDto wishlistDto)
    {
        _wishlistDtos.Add(wishlistDto);

        if (_wishlistDtos.Count >= _hrManagerService.EmployeesNumber)
        {
            var wishlists = new List<WishlistDto>(_wishlistDtos).
                Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees));
            
            _wishlistDtos.Clear();

            _hrManagerService.Wishlists = wishlists;
        }
    }
}
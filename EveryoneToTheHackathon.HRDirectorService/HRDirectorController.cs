using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Services;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneToTheHackathon.Controllers;

[ApiController]
[Route("api/hr_director")]
public class HrDirectorController(HRDirectorService hrDirectorService) : ControllerBase
{
    [HttpPost("employees")]
    public void GetEmployees([FromBody] List<EmployeeDto> employeeDtos)
    {
        var employees = employeeDtos.
            Select(eDto => new Employee(eDto.Id, eDto.Title, eDto.Name)).ToList();

        hrDirectorService.Employees = employees;
    }

    [HttpPost("wishlists")]
    public void GetWishlists([FromBody] List<WishlistDto> wishlistDtos)
    {
        var wishlists = wishlistDtos.
            Select(wDto => new Wishlist(wDto.EmployeeId, wDto.EmployeeTitle, wDto.DesiredEmployees)).ToList();

        hrDirectorService.Wishlists = wishlists;
    }
    
    [HttpPost("teams")]
    public void GetTeams([FromBody] List<TeamDto> teamDtos)
    {
        var teams = teamDtos.
            Select(tDto => new Team(
                new Employee(tDto.TeamLead.Id, tDto.TeamLead.Title, tDto.TeamLead.Name), 
                new Employee(tDto.Junior.Id, tDto.Junior.Title, tDto.Junior.Name))).ToList();

        hrDirectorService.Teams = teams;
    }
}

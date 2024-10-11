using System.Diagnostics;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeService(
    IOptions<ServiceSettings> settings, 
    IHackathonRepository hackathonRepository, 
    IEmployeeRepository employeeRepository, 
    IWishlistRepository wishlistRepository)
{
    public Employee Employee { get; } = settings.Value.Employee;
    public IEnumerable<Employee> ProbableTeammates { get; } = settings.Value.ProbableTeammates;
    
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;
    private IEmployeeRepository EmployeeRepository { get; } = employeeRepository;
    private IWishlistRepository WishlistRepository { get; } = wishlistRepository;

    public void SaveEmployeeAndWishlist(Wishlist wishlist, int hackathonId)
    {
        var hackathon = HackathonRepository.GetHackathonById(hackathonId);
        Debug.Assert(hackathon != null);
        
        ((List<Hackathon>)Employee.Hackathons).Add(hackathon);
        ((List<Wishlist>)Employee.Wishlists).Add(wishlist);
        ((List<Employee>)hackathon.Employees).Add(Employee);
        
        wishlist.Hackathon = hackathon;
        wishlist.HackathonId = hackathonId;
        
        ((List<Wishlist>)hackathon.Wishlists).Add(wishlist);
        
        EmployeeRepository.SaveEmployee(Employee, hackathon);
    }
} 
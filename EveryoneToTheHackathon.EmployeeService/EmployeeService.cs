using System.Diagnostics;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using EveryoneToTheHackathon.Repositories;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeService(
    ILogger<EmployeeBackgroundService> logger,
    IOptions<ServiceSettings> settings, 
    IHackathonRepository hackathonRepository, 
    IEmployeeRepository employeeRepository, 
    IWishlistRepository wishlistRepository,
    IBusControl busControl)
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
    
    public void SendEmployeeAndWishlistStoredAsyncViaMessage()
    {
        busControl.Publish(
            new EmployeeAndWishlistSent($"Employee 'Id = {Employee.Id} " +
                                        $"Title = {Employee.Title} " +
                                        $"Name = {Employee.Name}' " +
                                        $"has stored his data", 
                Employee.Id, Employee.Title, Employee.Name));
        logger.LogInformation("Employee has send his hackathon confirmation");
    }
} 
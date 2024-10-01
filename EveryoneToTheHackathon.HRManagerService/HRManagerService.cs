using System.Collections.Concurrent;
using EveryoneToTheHackathon.Entities;
using Microsoft.Extensions.Options;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerService(IOptions<ControllerSettings> settings, HRManager hrManager)
{
    public readonly int EmployeesNumber = settings.Value.EmployeesNumber;
    public HRManager HrManager { get; } = hrManager;

    //public List<Employee>? Employees { get; set; }
    //public List<Wishlist>? Wishlists { get; set; }
    public ConcurrentBag<Employee> Employees { get; set; } = [];
    public ConcurrentBag<Wishlist> Wishlists { get; set; } = [];

    public TaskCompletionSource<bool> HackathonStartedTcs = new();
    public TaskCompletionSource<bool> EmployeesGotTcs = new();
    public TaskCompletionSource<bool> WishlistsGotTcs = new();

    public void ResetAll()
    {
        HackathonStartedTcs = new TaskCompletionSource<bool>();
        EmployeesGotTcs = new TaskCompletionSource<bool>();
        WishlistsGotTcs = new TaskCompletionSource<bool>();
        
        Employees.Clear();
        Wishlists.Clear();
    }
}
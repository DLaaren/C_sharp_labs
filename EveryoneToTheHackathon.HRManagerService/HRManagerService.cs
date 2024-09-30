using System.Collections.Concurrent;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerService(HRManager hrManager)
{
    public HRManager HrManager { get; } = hrManager;

    public List<Employee>? Employees { get; set; }
    public List<Wishlist>? Wishlists { get; set; }
    public ConcurrentBag<Employee> EmployeesConcurrent = [];
    public ConcurrentBag<Wishlist> WishlistsConcurrent = [];
    
    public readonly TaskCompletionSource<bool> HackathonStartedTcs = new();
}
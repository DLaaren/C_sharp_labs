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

    public readonly TaskCompletionSource<bool> HackathonStartedTcs = new();
    public readonly TaskCompletionSource<bool> EmployeesGotTcs = new();
    public readonly TaskCompletionSource<bool> WishlistsGotTcs = new();
}
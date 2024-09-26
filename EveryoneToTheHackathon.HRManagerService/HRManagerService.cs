using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.HRManagerService;

public class HrManagerService(HRManager hrManager)
{
    public HRManager HrManager { get; } = hrManager;

    public List<Employee>? Employees { get; set; }
    public List<Wishlist>? Wishlists { get; set; }
}
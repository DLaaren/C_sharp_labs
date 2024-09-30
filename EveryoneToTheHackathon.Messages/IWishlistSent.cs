using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Messages;

public interface IWishlistSent
{
    int EmployeeId { get; }
    string EmployeeTitle { get; }
    Wishlist Wishlist { get; }
}
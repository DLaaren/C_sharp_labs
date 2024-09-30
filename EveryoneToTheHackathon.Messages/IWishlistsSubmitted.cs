using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Messages;

public interface IWishlistsSubmitted
{
    int EmployeeId { get; }
    string EmployeeTitle { get; }
    Wishlist Wishlist { get; }
}
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IWishlistRepository
{
    public void AddWishlist(Wishlist wishlist);
    public IEnumerable<Wishlist> GetWishlistByHackathonId(int hackathonId);
}
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IWishlistRepository
{
    public void AddWishlist(Wishlist wishlist);
    public void UpdateWishlist(Wishlist wishlist);
    public void SaveWishlist(Wishlist wishlist);
    public IEnumerable<Wishlist> GetWishlistByHackathonId(int hackathonId);
}
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Repositories;

public interface IWishlistRepository
{
    public Wishlist? GetWishlistById(int wishlistId);
    public IEnumerable<Wishlist> GetWishlists();
    public void AddWishlist(Wishlist wishlist);
    public void AddWishlists(IEnumerable<Wishlist> wishlists);
    public void UpdateWishlist(Wishlist wishlist);
    public void UpdateWishlists(IEnumerable<Wishlist> wishlists);
}
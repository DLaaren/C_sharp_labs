using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services;

public class WishlistService(AppDbContext dbContext) : IWishlistService
{
    private readonly AppDbContext _dbContext = dbContext;

    public Wishlist? GetWishlistById(int wishlistId)
    {
        return _dbContext.Wishlists.Find(wishlistId);
    }
    
    public IEnumerable<Wishlist> GetWishlists()
    {
        return _dbContext.Wishlists.ToList();
    }

    public void AddWishlist(Wishlist wishlist)
    {
        _dbContext.Add(wishlist);
        _dbContext.SaveChanges();
    }
    
    public void AddWishlists(IEnumerable<Wishlist> wishlists)
    {
        _dbContext.AddRange(wishlists);
        _dbContext.SaveChanges();
    }
    
    public void UpdateWishlist(Wishlist wishlist)
    {
        _dbContext.Update(wishlist);
        _dbContext.SaveChanges();
    }
    
    public void UpdateWishlists(IEnumerable<Wishlist> wishlists)
    {
        _dbContext.UpdateRange(wishlists);
        _dbContext.SaveChanges();
    }
}
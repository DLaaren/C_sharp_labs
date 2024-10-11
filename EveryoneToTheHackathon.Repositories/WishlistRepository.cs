using System.Diagnostics;
using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class WishlistRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : IWishlistRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddWishlist(Wishlist wishlist)
    {
        _dbContext.Entry(wishlist).State = EntityState.Added;
        
    }

    public void UpdateWishlist(Wishlist wishlist)
    {
        if (!_dbContext.Wishlists.Any(w => w.Id == wishlist.Id))
            AddWishlist(wishlist);
        else
            _dbContext.Update(wishlist);
    }

    public void SaveWishlist(Wishlist wishlist)
    {
        Debug.Assert(wishlist.Hackathon != null);
        _dbContext.Entry(wishlist.Hackathon).State = EntityState.Modified;
        UpdateWishlist(wishlist);
        _dbContext.SaveChanges();
    }
    
    public IEnumerable<Wishlist> GetWishlistByHackathonId(int hackathonId)
    {
        return _dbContext.Wishlists.Where(w => w.Hackathon != null && w.Hackathon.Id == hackathonId).ToList();
    }
}
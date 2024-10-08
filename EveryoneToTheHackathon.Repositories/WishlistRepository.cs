using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class WishlistRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : IWishlistRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddWishlist(Wishlist wishlist)
    {
        _dbContext.Add(wishlist);
        _dbContext.SaveChanges();
    }
    
    public IEnumerable<Wishlist> GetWishlistByHackathonId(int hackathonId)
    {
        return _dbContext.Wishlists.Where(w => w.Hackathon != null && w.Hackathon.Id == hackathonId).ToList();
    }
}
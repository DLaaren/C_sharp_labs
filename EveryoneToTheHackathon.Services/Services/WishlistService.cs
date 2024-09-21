using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services.Services;

public class WishlistService(AppDbContext dbContext) : IWishlistService
{
    private readonly AppDbContext _dbContext = dbContext;
   
    public async Task<IEnumerable<Wishlist>> GetWishlistsAsync()
    {
        return await _dbContext.Wishlists.ToListAsync();
    }
    
    public async Task AddWishlistsAsync(IEnumerable<Wishlist> wishlists)
    {
        _dbContext.AddRange(wishlists);
        await _dbContext.SaveChangesAsync();
    }
}
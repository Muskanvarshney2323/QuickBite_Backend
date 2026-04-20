using Microsoft.EntityFrameworkCore;
using QuickBite.Restaurant.Application.Interfaces;
using QuickBite.Restaurant.Infrastructure.Data;
using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Infrastructure.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantEntity> AddAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<IEnumerable<RestaurantEntity>> GetAllAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }

        public async Task<RestaurantEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }
    }
}
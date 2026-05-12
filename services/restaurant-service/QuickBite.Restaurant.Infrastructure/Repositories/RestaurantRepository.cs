using Microsoft.EntityFrameworkCore;
using QuickBite.Restaurant.Application.Interfaces;
using QuickBite.Restaurant.Infrastructure.Data;
using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Infrastructure.Repositories
{
    // This class implements database operations using EF Core
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        // Add new restaurant to database
        public async Task<RestaurantEntity> AddAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        // Fetch all restaurants
        public async Task<IEnumerable<RestaurantEntity>> GetAllAsync()
        {
            return await _context.Restaurants
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Fetch restaurant by id
        public async Task<RestaurantEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        }

        // Update restaurant details
        public async Task UpdateAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        // Delete restaurant
        public async Task DeleteAsync(RestaurantEntity restaurant)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }

        // Get restaurants by city (case insensitive)
        public async Task<IEnumerable<RestaurantEntity>> GetByCityAsync(string city)
        {
            return await _context.Restaurants
                .Where(r => r.City.ToLower() == city.ToLower())
                .ToListAsync();
        }

        // Get restaurants by cuisine
        public async Task<IEnumerable<RestaurantEntity>> GetByCuisineAsync(string cuisine)
        {
            return await _context.Restaurants
                .Where(r => r.Cuisine.ToLower() == cuisine.ToLower())
                .ToListAsync();
        }

        // Get restaurants by owner id
        public async Task<IEnumerable<RestaurantEntity>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Restaurants
                .Where(r => r.OwnerId == ownerId)
                .ToListAsync();
        }

        // Get only approved restaurants
        public async Task<IEnumerable<RestaurantEntity>> GetApprovedAsync()
        {
            return await _context.Restaurants
                .Where(r => r.IsApproved)
                .ToListAsync();
        }

        // Get only open restaurants
        public async Task<IEnumerable<RestaurantEntity>> GetOpenAsync()
        {
            return await _context.Restaurants
                .Where(r => r.IsOpen)
                .ToListAsync();
        }

        // Search restaurants by name
        public async Task<IEnumerable<RestaurantEntity>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllAsync();

            var keyword = name.Trim().ToLower();

            return await _context.Restaurants
                .Where(r => r.Name.ToLower().Contains(keyword) || r.Cuisine.ToLower().Contains(keyword))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
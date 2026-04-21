using Microsoft.EntityFrameworkCore;
using QuickBite.Menu.Domain.Entities;
using QuickBite.Menu.Domain.Interfaces;
using QuickBite.Menu.Infrastructure.Data;

namespace QuickBite.Menu.Infrastructure.Repositories
{
    // Repository implementation for menu category
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly MenuDbContext _context;

        // Constructor injection for DbContext
        public MenuCategoryRepository(MenuDbContext context)
        {
            _context = context;
        }

        // Returns all categories
        public async Task<IEnumerable<MenuCategory>> GetAllAsync()
        {
            return await _context.MenuCategories.ToListAsync();
        }

        // Returns one category by id
        public async Task<MenuCategory?> GetByIdAsync(Guid id)
        {
            return await _context.MenuCategories.FindAsync(id);
        }

        // Returns categories by restaurant id
        public async Task<IEnumerable<MenuCategory>> GetByRestaurantIdAsync(Guid restaurantId)
        {
            return await _context.MenuCategories
                .Where(category => category.RestaurantId == restaurantId)
                .ToListAsync();
        }

        // Adds new category into database
        public async Task AddAsync(MenuCategory category)
        {
            await _context.MenuCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Updates existing category
        public async Task UpdateAsync(MenuCategory category)
        {
            _context.MenuCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Deletes category
        public async Task DeleteAsync(MenuCategory category)
        {
            _context.MenuCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
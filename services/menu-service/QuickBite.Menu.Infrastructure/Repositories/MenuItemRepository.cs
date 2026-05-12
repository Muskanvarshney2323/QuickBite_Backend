using Microsoft.EntityFrameworkCore;
using QuickBite.Menu.Domain.Entities;
using QuickBite.Menu.Domain.Interfaces;
using QuickBite.Menu.Infrastructure.Data;

namespace QuickBite.Menu.Infrastructure.Repositories
{
    // Repository implementation for menu item
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MenuDbContext _context;

        // Constructor injection for DbContext
        public MenuItemRepository(MenuDbContext context)
        {
            _context = context;
        }

        // Returns all menu items
        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems.ToListAsync();
        }

        // Returns one item by id
        public async Task<MenuItem?> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems.FindAsync(id);
        }

        // Returns items by category id
        public async Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .Where(item => item.MenuCategoryId == categoryId)
                .ToListAsync();
        }

        // Adds new item into database
        public async Task AddAsync(MenuItem item)
        {
            await _context.MenuItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        // Updates existing item
        public async Task UpdateAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        // Deletes item
        public async Task DeleteAsync(MenuItem item)
        {
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
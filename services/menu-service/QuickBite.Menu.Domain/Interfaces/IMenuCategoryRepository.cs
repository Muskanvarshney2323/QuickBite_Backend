using QuickBite.Menu.Domain.Entities;

namespace QuickBite.Menu.Domain.Interfaces
{
    // Repository interface for menu category database operations
    public interface IMenuCategoryRepository
    {
        Task<IEnumerable<MenuCategory>> GetAllAsync();
        Task<MenuCategory?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuCategory>> GetByRestaurantIdAsync(Guid restaurantId);
        Task AddAsync(MenuCategory category);
        Task UpdateAsync(MenuCategory category);
        Task DeleteAsync(MenuCategory category);
    }
}
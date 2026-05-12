using QuickBite.Menu.Domain.Entities;

namespace QuickBite.Menu.Domain.Interfaces
{
    // Repository interface for menu item database operations
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuItem>> GetByCategoryIdAsync(Guid categoryId);
        Task AddAsync(MenuItem item);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(MenuItem item);
    }
}
using QuickBite.Menu.Application.DTOs.MenuItem;

namespace QuickBite.Menu.Application.Interfaces
{
    // Service interface for menu item business logic
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemResponseDto>> GetAllAsync();
        Task<MenuItemResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuItemResponseDto>> GetByCategoryIdAsync(Guid categoryId);

        // Returns menu items for one selected restaurant only.
        Task<IEnumerable<MenuItemResponseDto>> GetByRestaurantIdAsync(Guid restaurantId);

        Task<MenuItemResponseDto> CreateAsync(CreateMenuItemRequestDto request);
        Task<bool> UpdateAsync(Guid id, UpdateMenuItemRequestDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}

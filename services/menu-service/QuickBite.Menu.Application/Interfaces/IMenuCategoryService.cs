using QuickBite.Menu.Application.DTOs.MenuCategory;

namespace QuickBite.Menu.Application.Interfaces
{
    // Service interface for menu category business logic
    public interface IMenuCategoryService
    {
        Task<IEnumerable<MenuCategoryResponseDto>> GetAllAsync();
        Task<MenuCategoryResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MenuCategoryResponseDto>> GetByRestaurantIdAsync(Guid restaurantId);
        Task<MenuCategoryResponseDto> CreateAsync(CreateMenuCategoryRequestDto request);
        Task<bool> UpdateAsync(Guid id, UpdateMenuCategoryRequestDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
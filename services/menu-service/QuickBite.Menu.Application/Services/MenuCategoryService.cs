using QuickBite.Menu.Application.DTOs.MenuCategory;
using QuickBite.Menu.Application.Interfaces;
using QuickBite.Menu.Domain.Entities;
using QuickBite.Menu.Domain.Interfaces;

namespace QuickBite.Menu.Application.Services
{
    // Service class contains business logic for menu categories
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;

        // Constructor injection for repository
        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository)
        {
            _menuCategoryRepository = menuCategoryRepository;
        }

        // Returns all categories
        public async Task<IEnumerable<MenuCategoryResponseDto>> GetAllAsync()
        {
            var categories = await _menuCategoryRepository.GetAllAsync();

            return categories.Select(category => new MenuCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                RestaurantId = category.RestaurantId
            });
        }

        // Returns category by id
        public async Task<MenuCategoryResponseDto?> GetByIdAsync(Guid id)
        {
            var category = await _menuCategoryRepository.GetByIdAsync(id);

            if (category == null)
                return null;

            return new MenuCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                RestaurantId = category.RestaurantId
            };
        }

        // Returns categories by restaurant id
        public async Task<IEnumerable<MenuCategoryResponseDto>> GetByRestaurantIdAsync(Guid restaurantId)
        {
            var categories = await _menuCategoryRepository.GetByRestaurantIdAsync(restaurantId);

            return categories.Select(category => new MenuCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                RestaurantId = category.RestaurantId
            });
        }

        // Creates a new category
        public async Task<MenuCategoryResponseDto> CreateAsync(CreateMenuCategoryRequestDto request)
        {
            var category = new MenuCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                RestaurantId = request.RestaurantId
            };

            await _menuCategoryRepository.AddAsync(category);

            return new MenuCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                RestaurantId = category.RestaurantId
            };
        }

        // Updates an existing category
        public async Task<bool> UpdateAsync(Guid id, UpdateMenuCategoryRequestDto request)
        {
            var existingCategory = await _menuCategoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
                return false;

            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;

            await _menuCategoryRepository.UpdateAsync(existingCategory);
            return true;
        }

        // Deletes a category
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingCategory = await _menuCategoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
                return false;

            await _menuCategoryRepository.DeleteAsync(existingCategory);
            return true;
        }
    }
}
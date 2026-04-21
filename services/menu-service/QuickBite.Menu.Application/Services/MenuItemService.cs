using QuickBite.Menu.Application.DTOs.MenuItem;
using QuickBite.Menu.Application.Interfaces;
using QuickBite.Menu.Domain.Entities;
using QuickBite.Menu.Domain.Interfaces;

namespace QuickBite.Menu.Application.Services
{
    // Service class contains business logic for menu items
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        // Constructor injection for repository
        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        // Returns all menu items
        public async Task<IEnumerable<MenuItemResponseDto>> GetAllAsync()
        {
            var items = await _menuItemRepository.GetAllAsync();

            return items.Select(item => new MenuItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl,
                MenuCategoryId = item.MenuCategoryId
            });
        }

        // Returns menu item by id
        public async Task<MenuItemResponseDto?> GetByIdAsync(Guid id)
        {
            var item = await _menuItemRepository.GetByIdAsync(id);

            if (item == null)
                return null;

            return new MenuItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl,
                MenuCategoryId = item.MenuCategoryId
            };
        }

        // Returns menu items by category id
        public async Task<IEnumerable<MenuItemResponseDto>> GetByCategoryIdAsync(Guid categoryId)
        {
            var items = await _menuItemRepository.GetByCategoryIdAsync(categoryId);

            return items.Select(item => new MenuItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl,
                MenuCategoryId = item.MenuCategoryId
            });
        }

        // Creates a new menu item
        public async Task<MenuItemResponseDto> CreateAsync(CreateMenuItemRequestDto request)
        {
            var item = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailable = request.IsAvailable,
                ImageUrl = request.ImageUrl,
                MenuCategoryId = request.MenuCategoryId
            };

            await _menuItemRepository.AddAsync(item);

            return new MenuItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl,
                MenuCategoryId = item.MenuCategoryId
            };
        }

        // Updates an existing menu item
        public async Task<bool> UpdateAsync(Guid id, UpdateMenuItemRequestDto request)
        {
            var existingItem = await _menuItemRepository.GetByIdAsync(id);

            if (existingItem == null)
                return false;

            existingItem.Name = request.Name;
            existingItem.Description = request.Description;
            existingItem.Price = request.Price;
            existingItem.IsAvailable = request.IsAvailable;
            existingItem.ImageUrl = request.ImageUrl;
            existingItem.MenuCategoryId = request.MenuCategoryId;

            await _menuItemRepository.UpdateAsync(existingItem);
            return true;
        }

        // Deletes a menu item
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingItem = await _menuItemRepository.GetByIdAsync(id);

            if (existingItem == null)
                return false;

            await _menuItemRepository.DeleteAsync(existingItem);
            return true;
        }
    }
}
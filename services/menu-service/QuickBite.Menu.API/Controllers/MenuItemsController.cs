using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Menu.Application.DTOs.MenuItem;
using QuickBite.Menu.Application.Interfaces;

namespace QuickBite.Menu.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing menu items
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        // Constructor injection
        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// Get all menu items
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuItemService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _menuItemService.GetByIdAsync(id);

            if (item == null)
                return NotFound(new { message = "Menu item not found." });

            return Ok(item);
        }

        /// <summary>
        /// Get menu items by category ID
        /// </summary>
        [HttpGet("category/{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategoryId(Guid categoryId)
        {
            var items = await _menuItemService.GetByCategoryIdAsync(categoryId);
            return Ok(items);
        }

        /// <summary>
        /// Get menu items for one restaurant only.
        /// This fixes the issue where one restaurant's items were visible in every restaurant.
        /// </summary>
        [HttpGet("restaurant/{restaurantId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRestaurantId(Guid restaurantId)
        {
            var items = await _menuItemService.GetByRestaurantIdAsync(restaurantId);
            return Ok(items);
        }

        /// <summary>
        /// Create a new menu item
        /// </summary>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdItem = await _menuItemService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }

        /// <summary>
        /// Update an existing menu item
        /// </summary>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequestDto request)
        {
            var isUpdated = await _menuItemService.UpdateAsync(id, request);

            if (!isUpdated)
                return NotFound(new { message = "Menu item not found." });

            return Ok(new { message = "Menu item updated successfully." });
        }

        /// <summary>
        /// Delete a menu item
        /// </summary>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _menuItemService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound(new { message = "Menu item not found." });

            return Ok(new { message = "Menu item deleted successfully." });
        }
    }
}

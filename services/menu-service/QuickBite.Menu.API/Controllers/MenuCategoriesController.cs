using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Menu.Application.DTOs.MenuCategory;
using QuickBite.Menu.Application.Interfaces;

namespace QuickBite.Menu.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing menu categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategoryService;

        // Constructor injection for category service
        public MenuCategoriesController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        /// <summary>
        /// Get all menu categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _menuCategoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _menuCategoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { message = "Menu category not found." });

            return Ok(category);
        }

        /// <summary>
        /// Get categories by restaurant ID
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        [HttpGet("restaurant/{restaurantId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRestaurantId(Guid restaurantId)
        {
            var categories = await _menuCategoryService.GetByRestaurantIdAsync(restaurantId);
            return Ok(categories);
        }

        /// <summary>
        /// Create a new menu category
        /// </summary>
        /// <param name="request">Category details</param>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateMenuCategoryRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCategory = await _menuCategoryService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Update an existing menu category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="request">Updated details</param>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuCategoryRequestDto request)
        {
            var isUpdated = await _menuCategoryService.UpdateAsync(id, request);

            if (!isUpdated)
                return NotFound(new { message = "Menu category not found." });

            return Ok(new { message = "Menu category updated successfully." });
        }

        /// <summary>
        /// Delete a menu category
        /// </summary>
        /// <param name="id">Category ID</param>
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _menuCategoryService.DeleteAsync(id);

            if (!isDeleted)
                return NotFound(new { message = "Menu category not found." });

            return Ok(new { message = "Menu category deleted successfully." });
        }
    }
}
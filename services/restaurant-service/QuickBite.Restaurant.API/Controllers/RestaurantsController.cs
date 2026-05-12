using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Restaurant.Application.DTOs;
using QuickBite.Restaurant.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [Authorize(Roles = "Owner,RestaurantOwner,Admin")]
        [SwaggerOperation(Summary = "Create restaurant", Description = "Restaurant owner or admin can create a new restaurant.")]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantService.CreateRestaurantAsync(request);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, result);
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all restaurants", Description = "Returns all restaurants.")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var result = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(result);
        }

        // Extra route added because some frontend clients call /api/restaurants/all.
        [HttpGet("all")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all restaurants (alias)", Description = "Returns all restaurants. Alias endpoint for frontend compatibility.")]
        public async Task<IActionResult> GetAllRestaurantsAlias()
        {
            var result = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurant by ID", Description = "Returns restaurant details using restaurant ID.")]
        public async Task<IActionResult> GetRestaurantById(Guid id)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Restaurant not found." });

            return Ok(result);
        }

        [HttpGet("city/{city}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurants by city", Description = "Filters and returns all restaurants in a specified city.")]
        public async Task<IActionResult> GetRestaurantsByCity(string city)
        {
            var result = await _restaurantService.GetRestaurantsByCityAsync(city);
            return Ok(result);
        }

        [HttpGet("cuisine/{cuisine}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get restaurants by cuisine", Description = "Filters and returns all restaurants that serve a specified cuisine type.")]
        public async Task<IActionResult> GetRestaurantsByCuisine(string cuisine)
        {
            var result = await _restaurantService.GetRestaurantsByCuisineAsync(cuisine);
            return Ok(result);
        }

        [HttpGet("owner/{ownerId:guid}")]
        [Authorize(Roles = "Owner,RestaurantOwner,Admin")]
        [SwaggerOperation(Summary = "Get restaurants by owner ID", Description = "Returns all restaurants owned by a specific restaurant owner. Requires Owner, RestaurantOwner, or Admin role.")]
        public async Task<IActionResult> GetRestaurantsByOwner(Guid ownerId)
        {
            var result = await _restaurantService.GetRestaurantsByOwnerIdAsync(ownerId);
            return Ok(result);
        }

        [HttpGet("approved")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get approved restaurants", Description = "Returns all restaurants that have been approved by admin.")]
        public async Task<IActionResult> GetApprovedRestaurants()
        {
            var result = await _restaurantService.GetApprovedRestaurantsAsync();
            return Ok(result);
        }

        [HttpGet("open")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get open restaurants", Description = "Returns all restaurants that are currently open.")]
        public async Task<IActionResult> GetOpenRestaurants()
        {
            var result = await _restaurantService.GetOpenRestaurantsAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Search restaurants by name", Description = "Searches and returns restaurants matching the specified name.")]
        public async Task<IActionResult> SearchRestaurantsByName([FromQuery] string name)
        {
            var result = await _restaurantService.SearchRestaurantsByNameAsync(name);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Owner,RestaurantOwner,Admin")]
        [SwaggerOperation(Summary = "Update restaurant", Description = "Updates restaurant details. Only restaurant owner or admin can update. Requires Owner, RestaurantOwner, or Admin role.")]
        public async Task<IActionResult> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _restaurantService.UpdateRestaurantAsync(id, request);

            if (!updated)
                return NotFound(new { message = "Restaurant not found." });

            return Ok(new { message = "Restaurant updated successfully." });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,RestaurantOwner,Owner")]
        [SwaggerOperation(Summary = "Delete restaurant", Description = "Deletes a restaurant permanently. Only restaurant owner or admin can delete. Requires Admin, RestaurantOwner, or Owner role.")]
        public async Task<IActionResult> DeleteRestaurant(Guid id)
        {
            var deleted = await _restaurantService.DeleteRestaurantAsync(id);

            if (!deleted)
                return NotFound(new { message = "Restaurant not found." });

            return Ok(new { message = "Restaurant deleted successfully." });
        }

        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Approve restaurant", Description = "Approves a restaurant. Only admin can approve restaurants. Requires Admin role.")]
        public async Task<IActionResult> ApproveRestaurant(Guid id)
        {
            var approved = await _restaurantService.ApproveRestaurantAsync(id);

            if (!approved)
                return NotFound(new { message = "Restaurant not found." });

            return Ok(new { message = "Restaurant approved successfully." });
        }

        [HttpPut("{id:guid}/toggle-open")]
        [Authorize(Roles = "Owner,RestaurantOwner,Admin")]
        [SwaggerOperation(Summary = "Toggle restaurant open status", Description = "Toggles the open/closed status of a restaurant. Requires Owner, RestaurantOwner, or Admin role.")]
        public async Task<IActionResult> ToggleOpenStatus(Guid id)
        {
            var toggled = await _restaurantService.ToggleOpenStatusAsync(id);

            if (!toggled)
                return NotFound(new { message = "Restaurant not found." });

            return Ok(new { message = "Restaurant open status updated successfully." });
        }
    }
}

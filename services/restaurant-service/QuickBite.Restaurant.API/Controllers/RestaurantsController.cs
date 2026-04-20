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

        /// <summary>
        /// Create a new restaurant.
        /// </summary>
        [HttpPost]
        // [Authorize(Roles = "Owner")]
        [SwaggerOperation(
            Summary = "Create restaurant",
            Description = "This API is used to create a new restaurant."
        )]
        [SwaggerResponse(201, "Restaurant created successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestDto request)
        {
            var result = await _restaurantService.CreateRestaurantAsync(request);

            return CreatedAtAction(nameof(GetRestaurantById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Get all restaurants.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all restaurants",
            Description = "This API returns all restaurants."
        )]
        [SwaggerResponse(200, "Restaurants fetched successfully")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var result = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get restaurant by id.
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get restaurant by id",
            Description = "This API returns restaurant details using restaurant id."
        )]
        [SwaggerResponse(200, "Restaurant fetched successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> GetRestaurantById(Guid id)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Update restaurant details.
        /// </summary>
        [HttpPut("{id}")]
        // [Authorize(Roles = "Owner")]
        [SwaggerOperation(
            Summary = "Update restaurant",
            Description = "This API is used to update restaurant details."
        )]
        [SwaggerResponse(200, "Restaurant updated successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantRequestDto request)
        {
            var updated = await _restaurantService.UpdateRestaurantAsync(id, request);

            if (!updated)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(new { message = "Restaurant updated successfully." });
        }

        /// <summary>
        /// Delete restaurant by id.
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Delete restaurant",
            Description = "This API is used to delete a restaurant."
        )]
        [SwaggerResponse(200, "Restaurant deleted successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> DeleteRestaurant(Guid id)
        {
            var deleted = await _restaurantService.DeleteRestaurantAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(new { message = "Restaurant deleted successfully." });
        }

        /// <summary>
        /// Approve restaurant by id.
        /// </summary>
        [HttpPut("{id}/approve")]
        // [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Approve restaurant",
            Description = "This API is used to approve a restaurant."
        )]
        [SwaggerResponse(200, "Restaurant approved successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> ApproveRestaurant(Guid id)
        {
            var approved = await _restaurantService.ApproveRestaurantAsync(id);

            if (!approved)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(new { message = "Restaurant approved successfully." });
        }

        /// <summary>
        /// Toggle restaurant open/close status.
        /// </summary>
        [HttpPut("{id}/toggle-open")]
        // [Authorize(Roles = "Owner")]
        [SwaggerOperation(
            Summary = "Toggle restaurant open status",
            Description = "This API is used to open or close the restaurant."
        )]
        [SwaggerResponse(200, "Restaurant status updated successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> ToggleOpenStatus(Guid id)
        {
            var toggled = await _restaurantService.ToggleOpenStatusAsync(id);

            if (!toggled)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(new { message = "Restaurant open status updated successfully." });
        }
    }
}
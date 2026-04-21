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
            // Call service layer to create restaurant
            var result = await _restaurantService.CreateRestaurantAsync(request);

            // Return 201 Created with new restaurant data
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
            // Get all restaurant records
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
            // Find restaurant using id
            var result = await _restaurantService.GetRestaurantByIdAsync(id);

            // Return 404 if not found
            if (result == null)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get restaurants by city.
        /// </summary>
        [HttpGet("city/{city}")]
        [SwaggerOperation(
            Summary = "Get restaurants by city",
            Description = "This API returns all restaurants available in a given city."
        )]
        [SwaggerResponse(200, "Restaurants fetched successfully")]
        public async Task<IActionResult> GetRestaurantsByCity(string city)
        {
            // Filter restaurants by city name
            var result = await _restaurantService.GetRestaurantsByCityAsync(city);
            return Ok(result);
        }

        /// <summary>
        /// Get restaurants by cuisine.
        /// </summary>
        [HttpGet("cuisine/{cuisine}")]
        [SwaggerOperation(
            Summary = "Get restaurants by cuisine",
            Description = "This API returns restaurants based on cuisine type like Indian or Chinese."
        )]
        [SwaggerResponse(200, "Restaurants fetched successfully")]
        public async Task<IActionResult> GetRestaurantsByCuisine(string cuisine)
        {
            // Filter restaurants by cuisine
            var result = await _restaurantService.GetRestaurantsByCuisineAsync(cuisine);
            return Ok(result);
        }

        /// <summary>
        /// Get restaurants by owner id.
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        [SwaggerOperation(
            Summary = "Get restaurants by owner",
            Description = "This API returns all restaurants created by a specific owner."
        )]
        [SwaggerResponse(200, "Restaurants fetched successfully")]
        public async Task<IActionResult> GetRestaurantsByOwner(Guid ownerId)
        {
            // Filter restaurants by owner id
            var result = await _restaurantService.GetRestaurantsByOwnerIdAsync(ownerId);
            return Ok(result);
        }

        /// <summary>
        /// Get only approved restaurants.
        /// </summary>
        [HttpGet("approved")]
        [SwaggerOperation(
            Summary = "Get approved restaurants",
            Description = "This API returns only approved restaurants."
        )]
        [SwaggerResponse(200, "Approved restaurants fetched successfully")]
        public async Task<IActionResult> GetApprovedRestaurants()
        {
            // Get only restaurants approved by admin
            var result = await _restaurantService.GetApprovedRestaurantsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get only open restaurants.
        /// </summary>
        [HttpGet("open")]
        [SwaggerOperation(
            Summary = "Get open restaurants",
            Description = "This API returns only restaurants that are currently open."
        )]
        [SwaggerResponse(200, "Open restaurants fetched successfully")]
        public async Task<IActionResult> GetOpenRestaurants()
        {
            // Get restaurants whose IsOpen value is true
            var result = await _restaurantService.GetOpenRestaurantsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Search restaurants by name.
        /// </summary>
        [HttpGet("search")]
        [SwaggerOperation(
            Summary = "Search restaurant by name",
            Description = "This API searches restaurants by full or partial name."
        )]
        [SwaggerResponse(200, "Restaurants fetched successfully")]
        public async Task<IActionResult> SearchRestaurantsByName([FromQuery] string name)
        {
            // Search restaurant names using query parameter
            var result = await _restaurantService.SearchRestaurantsByNameAsync(name);
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
            // Update restaurant data
            var updated = await _restaurantService.UpdateRestaurantAsync(id, request);

            // Return 404 if restaurant is missing
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
            // Delete restaurant from database
            var deleted = await _restaurantService.DeleteRestaurantAsync(id);

            // Return 404 if restaurant is not found
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
            Description = "This API is used by admin to approve a restaurant."
        )]
        [SwaggerResponse(200, "Restaurant approved successfully")]
        [SwaggerResponse(404, "Restaurant not found")]
        public async Task<IActionResult> ApproveRestaurant(Guid id)
        {
            // Approve restaurant by changing IsApproved to true
            var approved = await _restaurantService.ApproveRestaurantAsync(id);

            // Return 404 if restaurant is missing
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
            // Toggle IsOpen value between true and false
            var toggled = await _restaurantService.ToggleOpenStatusAsync(id);

            // Return 404 if restaurant is not found
            if (!toggled)
            {
                return NotFound(new { message = "Restaurant not found." });
            }

            return Ok(new { message = "Restaurant open status updated successfully." });
        }
    }
}
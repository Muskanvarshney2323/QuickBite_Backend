using QuickBite.Restaurant.Application.DTOs;
using QuickBite.Restaurant.Application.Interfaces;
using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Application.Services
{
    // Service layer contains business logic for restaurant operations
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        // Create a new restaurant
        public async Task<RestaurantResponseDto> CreateRestaurantAsync(CreateRestaurantRequestDto request)
        {
            var restaurant = new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Cuisine = request.Cuisine.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim(),
                Phone = request.Phone.Trim(),
                OwnerId = request.OwnerId,
                // Demo-friendly default: created restaurants are visible to customers immediately.
                IsApproved = true,
                IsOpen = true,
                MinimumOrderAmount = request.MinimumOrderAmount,
                EstimatedDeliveryTimeInMinutes = request.EstimatedDeliveryTimeInMinutes,
                AverageRating = 0
            };

            var createdRestaurant = await _restaurantRepository.AddAsync(restaurant);

            return MapToResponseDto(createdRestaurant);
        }

        // Get all restaurants
        public async Task<IEnumerable<RestaurantResponseDto>> GetAllRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            return restaurants.Select(MapToResponseDto);
        }

        // Get one restaurant by its id
        public async Task<RestaurantResponseDto?> GetRestaurantByIdAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return null;

            return MapToResponseDto(restaurant);
        }

        // Update restaurant details
        public async Task<bool> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequestDto request)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return false;

            restaurant.Name = request.Name.Trim();
            restaurant.Description = request.Description.Trim();
            restaurant.Cuisine = request.Cuisine.Trim();
            restaurant.Address = request.Address.Trim();
            restaurant.City = request.City.Trim();
            restaurant.Phone = request.Phone.Trim();
            restaurant.MinimumOrderAmount = request.MinimumOrderAmount;
            restaurant.EstimatedDeliveryTimeInMinutes = request.EstimatedDeliveryTimeInMinutes;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

        // Delete restaurant
        public async Task<bool> DeleteRestaurantAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return false;

            await _restaurantRepository.DeleteAsync(restaurant);
            return true;
        }

        // Approve restaurant
        public async Task<bool> ApproveRestaurantAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return false;

            restaurant.IsApproved = true;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

        // Toggle restaurant open/close status
        public async Task<bool> ToggleOpenStatusAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return false;

            restaurant.IsOpen = !restaurant.IsOpen;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

        // Get restaurants by city
        public async Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByCityAsync(string city)
        {
            var restaurants = await _restaurantRepository.GetByCityAsync(city);
            return restaurants.Select(MapToResponseDto);
        }

        // Get restaurants by cuisine
        public async Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByCuisineAsync(string cuisine)
        {
            var restaurants = await _restaurantRepository.GetByCuisineAsync(cuisine);
            return restaurants.Select(MapToResponseDto);
        }

        // Get restaurants by owner id
        public async Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByOwnerIdAsync(Guid ownerId)
        {
            var restaurants = await _restaurantRepository.GetByOwnerIdAsync(ownerId);
            return restaurants.Select(MapToResponseDto);
        }

        // Get only approved restaurants
        public async Task<IEnumerable<RestaurantResponseDto>> GetApprovedRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetApprovedAsync();
            return restaurants.Select(MapToResponseDto);
        }

        // Get only open restaurants
        public async Task<IEnumerable<RestaurantResponseDto>> GetOpenRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetOpenAsync();
            return restaurants.Select(MapToResponseDto);
        }

        // Search restaurants by name
        public async Task<IEnumerable<RestaurantResponseDto>> SearchRestaurantsByNameAsync(string name)
        {
            var restaurants = await _restaurantRepository.SearchByNameAsync(name);
            return restaurants.Select(MapToResponseDto);
        }

        // Convert entity into response DTO
        private static RestaurantResponseDto MapToResponseDto(RestaurantEntity restaurant)
        {
            return new RestaurantResponseDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Cuisine = restaurant.Cuisine,
                Address = restaurant.Address,
                City = restaurant.City,
                Phone = restaurant.Phone,
                OwnerId = restaurant.OwnerId,
                IsApproved = restaurant.IsApproved,
                IsOpen = restaurant.IsOpen,
                MinimumOrderAmount = restaurant.MinimumOrderAmount,
                EstimatedDeliveryTimeInMinutes = restaurant.EstimatedDeliveryTimeInMinutes,
                AverageRating = restaurant.AverageRating
            };
        }
    }
}
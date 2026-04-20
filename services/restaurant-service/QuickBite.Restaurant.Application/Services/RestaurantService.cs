using QuickBite.Restaurant.Application.DTOs;
using QuickBite.Restaurant.Application.Interfaces;
using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<RestaurantResponseDto> CreateRestaurantAsync(CreateRestaurantRequestDto request)
        {
            var restaurant = new RestaurantEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Cuisine = request.Cuisine,
                Address = request.Address,
                City = request.City,
                Phone = request.Phone,
                OwnerId = request.OwnerId,
                IsApproved = false,
                IsOpen = true
            };

            var createdRestaurant = await _restaurantRepository.AddAsync(restaurant);

            return MapToResponseDto(createdRestaurant);
        }

        public async Task<IEnumerable<RestaurantResponseDto>> GetAllRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            return restaurants.Select(MapToResponseDto);
        }

        public async Task<RestaurantResponseDto?> GetRestaurantByIdAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return null;
            }

            return MapToResponseDto(restaurant);
        }

        public async Task<bool> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequestDto request)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return false;
            }

            restaurant.Name = request.Name;
            restaurant.Description = request.Description;
            restaurant.Cuisine = request.Cuisine;
            restaurant.Address = request.Address;
            restaurant.City = request.City;
            restaurant.Phone = request.Phone;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

        public async Task<bool> DeleteRestaurantAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return false;
            }

            await _restaurantRepository.DeleteAsync(restaurant);
            return true;
        }

        public async Task<bool> ApproveRestaurantAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return false;
            }

            restaurant.IsApproved = true;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

        public async Task<bool> ToggleOpenStatusAsync(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return false;
            }

            restaurant.IsOpen = !restaurant.IsOpen;

            await _restaurantRepository.UpdateAsync(restaurant);
            return true;
        }

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
                IsOpen = restaurant.IsOpen
            };
        }
    }
}
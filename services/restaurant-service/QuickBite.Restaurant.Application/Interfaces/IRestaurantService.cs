using QuickBite.Restaurant.Application.DTOs;

namespace QuickBite.Restaurant.Application.Interfaces
{
    public interface IRestaurantService
    {
        Task<RestaurantResponseDto> CreateRestaurantAsync(CreateRestaurantRequestDto request);

        Task<IEnumerable<RestaurantResponseDto>> GetAllRestaurantsAsync();

        Task<RestaurantResponseDto?> GetRestaurantByIdAsync(Guid id);

        Task<bool> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequestDto request);

        Task<bool> DeleteRestaurantAsync(Guid id);

        Task<bool> ApproveRestaurantAsync(Guid id);

        Task<bool> ToggleOpenStatusAsync(Guid id);

        Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByCityAsync(string city);

        Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByCuisineAsync(string cuisine);

        Task<IEnumerable<RestaurantResponseDto>> GetRestaurantsByOwnerIdAsync(Guid ownerId);

        Task<IEnumerable<RestaurantResponseDto>> GetApprovedRestaurantsAsync();

        Task<IEnumerable<RestaurantResponseDto>> GetOpenRestaurantsAsync();

        Task<IEnumerable<RestaurantResponseDto>> SearchRestaurantsByNameAsync(string name);
    }
}
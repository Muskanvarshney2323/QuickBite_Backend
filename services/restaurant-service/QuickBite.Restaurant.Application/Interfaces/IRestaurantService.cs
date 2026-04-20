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
    }
}
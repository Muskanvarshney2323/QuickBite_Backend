using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Application.Interfaces
{
    public interface IRestaurantRepository
    {
        Task<RestaurantEntity> AddAsync(RestaurantEntity restaurant);

        Task<IEnumerable<RestaurantEntity>> GetAllAsync();

        Task<RestaurantEntity?> GetByIdAsync(Guid id);

        Task UpdateAsync(RestaurantEntity restaurant);

        Task DeleteAsync(RestaurantEntity restaurant);
    }
}
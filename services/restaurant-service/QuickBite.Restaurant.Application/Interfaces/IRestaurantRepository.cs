using RestaurantEntity = QuickBite.Restaurant.Domain.Entities.Restaurant;

namespace QuickBite.Restaurant.Application.Interfaces
{
    // Repository interface defines all database operations for Restaurant
    public interface IRestaurantRepository
    {
        // Add new restaurant to database
        Task<RestaurantEntity> AddAsync(RestaurantEntity restaurant);

        // Get all restaurants
        Task<IEnumerable<RestaurantEntity>> GetAllAsync();

        // Get restaurant by id
        Task<RestaurantEntity?> GetByIdAsync(Guid id);

        // Update restaurant
        Task UpdateAsync(RestaurantEntity restaurant);

        // Delete restaurant
        Task DeleteAsync(RestaurantEntity restaurant);

        // Get restaurants by city
        Task<IEnumerable<RestaurantEntity>> GetByCityAsync(string city);

        // Get restaurants by cuisine
        Task<IEnumerable<RestaurantEntity>> GetByCuisineAsync(string cuisine);

        // Get restaurants of a specific owner
        Task<IEnumerable<RestaurantEntity>> GetByOwnerIdAsync(Guid ownerId);

        // Get only approved restaurants
        Task<IEnumerable<RestaurantEntity>> GetApprovedAsync();

        // Get only open restaurants
        Task<IEnumerable<RestaurantEntity>> GetOpenAsync();

        // Search restaurants by name (partial match)
        Task<IEnumerable<RestaurantEntity>> SearchByNameAsync(string name);
    }
}
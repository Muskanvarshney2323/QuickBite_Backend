// Alias so "ReviewEntity" always means the class,
// not the "QuickBite.Review" namespace.
using ReviewEntity = QuickBite.Review.Domain.Entities.Review;

namespace QuickBite.Review.Application.Interfaces;

/// <summary>
/// Repository contract for saving and loading reviews from the database.
/// </summary>
public interface IReviewRepository
{
    // Find a review by its id.
    Task<ReviewEntity?> FindByIdAsync(Guid reviewId);

    // Find the review for a given order (returns null if none exists).
    Task<ReviewEntity?> FindByOrderIdAsync(Guid orderId);

    // All reviews for a restaurant.
    Task<IReadOnlyList<ReviewEntity>> FindByRestaurantIdAsync(Guid restaurantId);

    // All reviews by a customer.
    Task<IReadOnlyList<ReviewEntity>> FindByCustomerIdAsync(Guid customerId);

    // All reviews for a delivery agent.
    Task<IReadOnlyList<ReviewEntity>> FindByAgentIdAsync(Guid agentId);

    // Average food rating for a restaurant.
    Task<double> AvgFoodRatingByRestaurantIdAsync(Guid restaurantId);

    // Average delivery rating for an agent.
    Task<double> AvgDeliveryRatingByAgentIdAsync(Guid agentId);

    // Total number of reviews for a restaurant.
    Task<int> CountByRestaurantIdAsync(Guid restaurantId);

    // Whether a review already exists for the given order.
    Task<bool> ExistsByOrderIdAsync(Guid orderId);

    // All reviews in the system.
    Task<IReadOnlyList<ReviewEntity>> FindAllAsync();

    // Save a new review.
    Task AddReviewAsync(ReviewEntity review);

    // Mark an existing review as updated.
    void UpdateReview(ReviewEntity review);

    // Remove a review.
    Task<bool> DeleteReviewAsync(Guid reviewId);

    // Save all pending changes.
    Task SaveChangesAsync();
}

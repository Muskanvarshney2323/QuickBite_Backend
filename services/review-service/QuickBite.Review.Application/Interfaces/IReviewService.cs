using QuickBite.Review.Application.DTOs;

namespace QuickBite.Review.Application.Interfaces;

/// <summary>
/// Service contract for review business logic.
/// Methods mirror the Review-Service class-diagram spec:
/// AddReview, GetByRestaurant, GetByCustomer, GetByOrder, GetByAgent,
/// UpdateReview, DeleteReview, GetAvgFoodRating, GetAvgDeliveryRating, GetAllReviews.
/// </summary>
public interface IReviewService
{
    // Add a new review for a completed order.
    Task<ReviewResponseDto?> AddReviewAsync(AddReviewRequestDto request);

    // Get all reviews for a restaurant.
    Task<IReadOnlyList<ReviewResponseDto>> GetByRestaurantAsync(Guid restaurantId);

    // Get all reviews by a customer.
    Task<IReadOnlyList<ReviewResponseDto>> GetByCustomerAsync(Guid customerId);

    // Get the review for a specific order.
    Task<ReviewResponseDto?> GetByOrderAsync(Guid orderId);

    // Get all reviews for a delivery agent.
    Task<IReadOnlyList<ReviewResponseDto>> GetByAgentAsync(Guid agentId);

    // Update ratings / comment of an existing review.
    Task<ReviewResponseDto?> UpdateReviewAsync(Guid reviewId, UpdateReviewRequestDto request);

    // Delete a review.
    Task<bool> DeleteReviewAsync(Guid reviewId);

    // Get the average food rating for a restaurant.
    Task<double> GetAvgFoodRatingAsync(Guid restaurantId);

    // Get the average delivery rating for an agent.
    Task<double> GetAvgDeliveryRatingAsync(Guid agentId);

    // Get every review in the system.
    Task<IReadOnlyList<ReviewResponseDto>> GetAllReviewsAsync();
}

using QuickBite.Review.Application.DTOs;
using QuickBite.Review.Application.Interfaces;

// Alias so "ReviewEntity" always means the class,
// not the "QuickBite.Review" namespace.
using ReviewEntity = QuickBite.Review.Domain.Entities.Review;

namespace QuickBite.Review.Application.Services;

/// <summary>
/// Business logic for reviews.
/// Handles add, get (by order/restaurant/customer/agent/all), update, delete,
/// and average rating lookups.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    // Add a new review. If the order already has a review, return null.
    public async Task<ReviewResponseDto?> AddReviewAsync(AddReviewRequestDto request)
    {
        // Only one review per order.
        var alreadyExists = await _repository.ExistsByOrderIdAsync(request.OrderId);
        if (alreadyExists) return null;

        var review = new ReviewEntity
        {
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            AgentId = request.AgentId,
            FoodRating = request.FoodRating,
            DeliveryRating = request.DeliveryRating,
            Comment = request.Comment,
            ReviewDate = DateTime.UtcNow,
            IsVerified = false
        };

        await _repository.AddReviewAsync(review);
        await _repository.SaveChangesAsync();

        return MapToResponse(review);
    }

    // All reviews for a restaurant.
    public async Task<IReadOnlyList<ReviewResponseDto>> GetByRestaurantAsync(Guid restaurantId)
    {
        var reviews = await _repository.FindByRestaurantIdAsync(restaurantId);
        return reviews.Select(MapToResponse).ToList();
    }

    // All reviews by a customer.
    public async Task<IReadOnlyList<ReviewResponseDto>> GetByCustomerAsync(Guid customerId)
    {
        var reviews = await _repository.FindByCustomerIdAsync(customerId);
        return reviews.Select(MapToResponse).ToList();
    }

    // The review for a specific order (null if none).
    public async Task<ReviewResponseDto?> GetByOrderAsync(Guid orderId)
    {
        var review = await _repository.FindByOrderIdAsync(orderId);
        return review is null ? null : MapToResponse(review);
    }

    // All reviews for a delivery agent.
    public async Task<IReadOnlyList<ReviewResponseDto>> GetByAgentAsync(Guid agentId)
    {
        var reviews = await _repository.FindByAgentIdAsync(agentId);
        return reviews.Select(MapToResponse).ToList();
    }

    // Update the ratings and comment of an existing review.
    public async Task<ReviewResponseDto?> UpdateReviewAsync(Guid reviewId, UpdateReviewRequestDto request)
    {
        var review = await _repository.FindByIdAsync(reviewId);
        if (review is null) return null;

        review.FoodRating = request.FoodRating;
        review.DeliveryRating = request.DeliveryRating;
        review.Comment = request.Comment;

        _repository.UpdateReview(review);
        await _repository.SaveChangesAsync();

        return MapToResponse(review);
    }

    // Delete a review.
    public async Task<bool> DeleteReviewAsync(Guid reviewId)
    {
        var deleted = await _repository.DeleteReviewAsync(reviewId);
        if (!deleted) return false;

        await _repository.SaveChangesAsync();
        return true;
    }

    // Average food rating for a restaurant.
    public async Task<double> GetAvgFoodRatingAsync(Guid restaurantId)
    {
        return await _repository.AvgFoodRatingByRestaurantIdAsync(restaurantId);
    }

    // Average delivery rating for an agent.
    public async Task<double> GetAvgDeliveryRatingAsync(Guid agentId)
    {
        return await _repository.AvgDeliveryRatingByAgentIdAsync(agentId);
    }

    // Every review in the system.
    public async Task<IReadOnlyList<ReviewResponseDto>> GetAllReviewsAsync()
    {
        var reviews = await _repository.FindAllAsync();
        return reviews.Select(MapToResponse).ToList();
    }

    // Convert an entity into the response DTO.
    private static ReviewResponseDto MapToResponse(ReviewEntity review)
    {
        return new ReviewResponseDto
        {
            ReviewId = review.Id,
            OrderId = review.OrderId,
            CustomerId = review.CustomerId,
            RestaurantId = review.RestaurantId,
            AgentId = review.AgentId,
            FoodRating = review.FoodRating,
            DeliveryRating = review.DeliveryRating,
            Comment = review.Comment,
            ReviewDate = review.ReviewDate,
            IsVerified = review.IsVerified
        };
    }
}

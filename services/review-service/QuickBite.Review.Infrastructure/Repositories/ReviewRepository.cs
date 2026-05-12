using Microsoft.EntityFrameworkCore;
using QuickBite.Review.Application.Interfaces;
using QuickBite.Review.Infrastructure.Data;

// Alias so "ReviewEntity" always means the class,
// not the "QuickBite.Review" namespace.
using ReviewEntity = QuickBite.Review.Domain.Entities.Review;

namespace QuickBite.Review.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the review repository.
/// All database queries live here.
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly ReviewDbContext _context;

    public ReviewRepository(ReviewDbContext context)
    {
        _context = context;
    }

    // Find a review by its id.
    public async Task<ReviewEntity?> FindByIdAsync(Guid reviewId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }

    // Find the single review for an order.
    public async Task<ReviewEntity?> FindByOrderIdAsync(Guid orderId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.OrderId == orderId);
    }

    // All reviews for a restaurant.
    public async Task<IReadOnlyList<ReviewEntity>> FindByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .ToListAsync();
    }

    // All reviews by a customer.
    public async Task<IReadOnlyList<ReviewEntity>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _context.Reviews
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();
    }

    // All reviews for a delivery agent.
    public async Task<IReadOnlyList<ReviewEntity>> FindByAgentIdAsync(Guid agentId)
    {
        return await _context.Reviews
            .Where(r => r.AgentId == agentId)
            .ToListAsync();
    }

    // Average food rating for a restaurant. Returns 0 if no reviews yet.
    public async Task<double> AvgFoodRatingByRestaurantIdAsync(Guid restaurantId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .ToListAsync();

        if (reviews.Count == 0) return 0;

        return reviews.Average(r => r.FoodRating);
    }

    // Average delivery rating for an agent. Returns 0 if no reviews yet.
    public async Task<double> AvgDeliveryRatingByAgentIdAsync(Guid agentId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.AgentId == agentId)
            .ToListAsync();

        if (reviews.Count == 0) return 0;

        return reviews.Average(r => r.DeliveryRating);
    }

    // Count of reviews for a restaurant.
    public async Task<int> CountByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Reviews.CountAsync(r => r.RestaurantId == restaurantId);
    }

    // True if any review exists for the given order.
    public async Task<bool> ExistsByOrderIdAsync(Guid orderId)
    {
        return await _context.Reviews.AnyAsync(r => r.OrderId == orderId);
    }

    // All reviews.
    public async Task<IReadOnlyList<ReviewEntity>> FindAllAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    // Add a new review to the DbSet (not saved until SaveChanges).
    public async Task AddReviewAsync(ReviewEntity review)
    {
        await _context.Reviews.AddAsync(review);
    }

    // Mark the review as updated so EF tracks the changes.
    public void UpdateReview(ReviewEntity review)
    {
        _context.Reviews.Update(review);
    }

    // Delete a review (returns false if it didn't exist).
    public async Task<bool> DeleteReviewAsync(Guid reviewId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        if (review is null) return false;

        _context.Reviews.Remove(review);
        return true;
    }

    // Commit all pending changes to the database.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

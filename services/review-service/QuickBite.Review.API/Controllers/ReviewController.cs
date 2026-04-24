using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Review.Application.DTOs;
using QuickBite.Review.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Review.API.Controllers;

/// <summary>
/// API endpoints for reviews.
/// Route: /api/v1/reviews
/// </summary>
[ApiController]
[Route("api/v1/reviews")]
[Authorize]
[SwaggerTag("Review operations: add, get (by restaurant/customer/order/agent/all), update, delete, average ratings")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // Add a new review for a completed order.
    [HttpPost]
    [SwaggerOperation(Summary = "Add review")]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequestDto request)
    {
        var result = await _reviewService.AddReviewAsync(request);
        if (result is null) return BadRequest("This order already has a review.");
        return CreatedAtAction(nameof(GetByOrder), new { orderId = result.OrderId }, result);
    }

    // Get all reviews for a restaurant.
    [HttpGet("restaurant/{restaurantId:guid}")]
    [SwaggerOperation(Summary = "Get reviews by restaurant")]
    public async Task<IActionResult> GetByRestaurant(Guid restaurantId)
    {
        var result = await _reviewService.GetByRestaurantAsync(restaurantId);
        return Ok(result);
    }

    // Get all reviews written by a customer.
    [HttpGet("customer/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get reviews by customer")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var result = await _reviewService.GetByCustomerAsync(customerId);
        return Ok(result);
    }

    // Get the review for a specific order.
    [HttpGet("order/{orderId:guid}")]
    [SwaggerOperation(Summary = "Get review by order")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await _reviewService.GetByOrderAsync(orderId);
        if (result is null) return NotFound("No review found for this order.");
        return Ok(result);
    }

    // Get all reviews for a delivery agent.
    [HttpGet("agent/{agentId:guid}")]
    [SwaggerOperation(Summary = "Get reviews by agent")]
    public async Task<IActionResult> GetByAgent(Guid agentId)
    {
        var result = await _reviewService.GetByAgentAsync(agentId);
        return Ok(result);
    }

    // Get every review in the system.
    [HttpGet("all")]
    [SwaggerOperation(Summary = "Get all reviews")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewService.GetAllReviewsAsync();
        return Ok(result);
    }

    // Update ratings / comment of a review.
    [HttpPut("{reviewId:guid}")]
    [SwaggerOperation(Summary = "Update review")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewRequestDto request)
    {
        var result = await _reviewService.UpdateReviewAsync(reviewId, request);
        if (result is null) return NotFound("Review not found.");
        return Ok(result);
    }

    // Delete a review.
    [HttpDelete("{reviewId:guid}")]
    [SwaggerOperation(Summary = "Delete review")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var deleted = await _reviewService.DeleteReviewAsync(reviewId);
        if (!deleted) return NotFound("Review not found.");
        return NoContent();
    }

    // Average food rating for a restaurant.
    [HttpGet("avgFood/{restaurantId:guid}")]
    [SwaggerOperation(Summary = "Get average food rating for a restaurant")]
    public async Task<IActionResult> GetAvgFood(Guid restaurantId)
    {
        var avg = await _reviewService.GetAvgFoodRatingAsync(restaurantId);
        return Ok(new { restaurantId, avgFoodRating = avg });
    }

    // Average delivery rating for an agent.
    [HttpGet("avgDelivery/{agentId:guid}")]
    [SwaggerOperation(Summary = "Get average delivery rating for an agent")]
    public async Task<IActionResult> GetAvgDelivery(Guid agentId)
    {
        var avg = await _reviewService.GetAvgDeliveryRatingAsync(agentId);
        return Ok(new { agentId, avgDeliveryRating = avg });
    }
}

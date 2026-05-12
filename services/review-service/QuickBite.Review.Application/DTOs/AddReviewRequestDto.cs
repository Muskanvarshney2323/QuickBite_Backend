namespace QuickBite.Review.Application.DTOs;

/// <summary>
/// Request DTO used when a customer submits a new review for an order.
/// </summary>
public class AddReviewRequestDto
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid AgentId { get; set; }

    /// <summary>Food rating (1 to 5).</summary>
    public int FoodRating { get; set; }

    /// <summary>Delivery rating (1 to 5).</summary>
    public int DeliveryRating { get; set; }

    /// <summary>Optional comment.</summary>
    public string Comment { get; set; } = string.Empty;
}

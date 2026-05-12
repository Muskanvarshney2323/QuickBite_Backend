namespace QuickBite.Review.Application.DTOs;

/// <summary>
/// Response DTO returned by all review endpoints.
/// </summary>
public class ReviewResponseDto
{
    public Guid ReviewId { get; set; }
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid AgentId { get; set; }
    public int FoodRating { get; set; }
    public int DeliveryRating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
    public bool IsVerified { get; set; }
}

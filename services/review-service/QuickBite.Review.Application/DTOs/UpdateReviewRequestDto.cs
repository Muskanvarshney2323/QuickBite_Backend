namespace QuickBite.Review.Application.DTOs;

/// <summary>
/// Request DTO for updating an existing review's ratings or comment.
/// </summary>
public class UpdateReviewRequestDto
{
    public int FoodRating { get; set; }
    public int DeliveryRating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

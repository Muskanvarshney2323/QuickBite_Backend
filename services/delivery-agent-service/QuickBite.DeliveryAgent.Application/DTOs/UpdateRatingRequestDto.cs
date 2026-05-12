namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO used by the customer app to rate an agent after delivery.
/// Rating must be between 0.0 and 5.0.
/// </summary>
public class UpdateRatingRequestDto
{
    public double NewRating { get; set; }
}

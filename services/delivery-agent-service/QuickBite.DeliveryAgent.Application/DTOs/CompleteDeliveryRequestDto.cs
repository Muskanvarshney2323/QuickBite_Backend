namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO used when the agent marks a delivery as completed.
/// This increments TotalDeliveries and flips IsAvailable back on.
/// </summary>
public class CompleteDeliveryRequestDto
{
    /// <summary>Order that was just delivered.</summary>
    public Guid OrderId { get; set; }
}

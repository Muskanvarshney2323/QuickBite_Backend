namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO for the "reorder from history" feature.
/// The customer picks a previous order and the service places a new order
/// using the same restaurant, items, and delivery details.
/// </summary>
public class ReorderRequestDto
{
    public Guid CustomerId { get; set; }

    public Guid PreviousOrderId { get; set; }

    /// <summary>Optional delivery address override; defaults to the previous order's address.</summary>
    public string? DeliveryAddress { get; set; }

    /// <summary>Optional payment mode override; defaults to the previous order's mode.</summary>
    public Domain.Enums.PaymentMode? ModeOfPayment { get; set; }
}

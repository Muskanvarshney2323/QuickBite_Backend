namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO for cancelling an order. Cancellation triggers a refund
/// when the original payment was prepaid (UPI / Card / Wallet / Net Banking).
/// COD orders are simply marked CANCELLED with no refund.
/// </summary>
public class CancelOrderRequestDto
{
    public string? Reason { get; set; }
}

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to refund a previously captured payment.
/// </summary>
public class RefundRequestDto
{
    public string? Reason { get; set; }
}

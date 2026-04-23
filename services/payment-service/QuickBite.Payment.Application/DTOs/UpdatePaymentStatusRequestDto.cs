using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to update a payment's lifecycle status.
/// </summary>
public class UpdatePaymentStatusRequestDto
{
    public PaymentStatus NewStatus { get; set; }
}

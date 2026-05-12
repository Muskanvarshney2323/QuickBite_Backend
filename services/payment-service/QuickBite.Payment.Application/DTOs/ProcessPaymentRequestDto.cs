using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to process a payment for an order.
/// One Payment record will be created per OrderId.
/// </summary>
public class ProcessPaymentRequestDto
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMode Mode { get; set; } = PaymentMode.UPI;

    public string Currency { get; set; } = "INR";
}

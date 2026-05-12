using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Response DTO for a Payment record.
/// </summary>
public class PaymentResponseDto
{
    public Guid PaymentId { get; set; }

    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public PaymentMode Mode { get; set; }

    public string? TransactionId { get; set; }

    public string Currency { get; set; } = "INR";

    public DateTime? PaidAt { get; set; }

    public DateTime? RefundedAt { get; set; }
}

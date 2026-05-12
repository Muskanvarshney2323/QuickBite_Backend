using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to top up a customer's wallet.
/// </summary>
public class AddToWalletRequestDto
{
    public Guid CustomerId { get; set; }

    public decimal Amount { get; set; }

    /// <summary>How the top-up was funded (defaults to UPI).</summary>
    public PaymentMode FundingMode { get; set; } = PaymentMode.UPI;

    /// <summary>Optional upstream reference (gateway transaction id).</summary>
    public string? Reference { get; set; }
}

using QuickBite.Payment.Domain.Common;
using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Domain.Entities;

/// <summary>
/// Records a payment attempt against an order.
/// Each Payment is linked to exactly one Order. Stores the amount,
/// payment mode, transaction id, currency and status.
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>The order this payment is for. One payment per order.</summary>
    public Guid OrderId { get; set; }

    /// <summary>The customer who made the payment.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Amount transacted.</summary>
    public decimal Amount { get; set; }

    /// <summary>Current status of the payment (PENDING / PAID / REFUNDED / FAILED).</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

    /// <summary>How the customer paid (CASH_ON_DELIVERY / UPI / CARD / WALLET / NET_BANKING).</summary>
    public PaymentMode Mode { get; set; }

    /// <summary>Identifier returned by the upstream gateway (Razorpay / Stripe / wallet ledger).</summary>
    public string? TransactionId { get; set; }

    /// <summary>ISO 4217 currency code, e.g. "INR".</summary>
    public string Currency { get; set; } = "INR";

    /// <summary>Timestamp the payment was successfully captured (null until PAID).</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Timestamp the payment was refunded (null unless REFUNDED).</summary>
    public DateTime? RefundedAt { get; set; }

    /// <summary>Updates the payment's lifecycle status. Domain method declared on the spec class diagram.</summary>
    public void UpdatePaymentStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.", nameof(newStatus));

        if (!Enum.TryParse<PaymentStatus>(newStatus, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Unknown payment status '{newStatus}'.", nameof(newStatus));

        Status = parsed;

        if (parsed == PaymentStatus.PAID && PaidAt is null)
            PaidAt = DateTime.UtcNow;

        if (parsed == PaymentStatus.REFUNDED && RefundedAt is null)
            RefundedAt = DateTime.UtcNow;
    }
}

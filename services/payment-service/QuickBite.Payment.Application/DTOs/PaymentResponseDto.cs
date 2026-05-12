using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Response DTO for a Payment record.
/// Returned after processing a payment. Contains the payment status, transaction details,
/// and timestamps for tracking payment lifecycle.
///
/// Response format:
/// {
///   "paymentId": "guid",
///   "orderId": "guid",
///   "customerId": "guid",
///   "amount": decimal,
///   "status": "PENDING|PAID|FAILED|REFUNDED",
///   "mode": 1-4 (CASH_ON_DELIVERY, CARD, UPI, WALLET),
///   "transactionId": "string-or-null",
///   "currency": "string",
///   "paidAt": "datetime-or-null",
///   "refundedAt": "datetime-or-null"
/// }
/// </summary>
public class PaymentResponseDto
{
    /// <summary>Unique identifier for this payment record.</summary>
    public Guid PaymentId { get; set; }

    /// <summary>The order ID associated with this payment.</summary>
    public Guid OrderId { get; set; }

    /// <summary>The customer ID who made this payment.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Payment amount charged.</summary>
    public decimal Amount { get; set; }

    /// <summary>Current payment status: PENDING, PAID, FAILED, or REFUNDED.</summary>
    public PaymentStatus Status { get; set; }

    /// <summary>Payment mode used: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET.</summary>
    public PaymentMode Mode { get; set; }

    /// <summary>Transaction ID from the payment gateway (Razorpay/Stripe). Null for CASH_ON_DELIVERY.</summary>
    public string? TransactionId { get; set; }

    /// <summary>Currency code. Typically "INR".</summary>
    public string Currency { get; set; } = "INR";

    /// <summary>Timestamp when payment was marked PAID. Null if not yet paid.</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Timestamp when payment was refunded. Null if not refunded.</summary>
    public DateTime? RefundedAt { get; set; }
}

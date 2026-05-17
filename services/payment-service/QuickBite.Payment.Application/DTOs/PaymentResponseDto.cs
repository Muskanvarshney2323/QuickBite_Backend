// Import for payment status and mode enums
using QuickBite.Payment.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Payment.Application.DTOs;

// ========================= PAYMENT RESPONSE DTO =========================
/// <summary>
/// PaymentResponseDto: Response DTO for Payment record details
/// Used in response to payment processing endpoints
/// Contains payment status, transaction details, and timestamps
/// 
/// Response body format:
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
    // ========================= PAYMENT ID =========================
    /// <summary>Unique identifier for this payment record.</summary>
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Database primary key for payment record
    public Guid PaymentId { get; set; }

    // ========================= ORDER ID =========================
    /// <summary>The order ID associated with this payment.</summary>
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links payment to the order it's for
    public Guid OrderId { get; set; }

    // ========================= CUSTOMER ID =========================
    /// <summary>The customer ID who made this payment.</summary>
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Identifies which customer's account payment goes to
    public Guid CustomerId { get; set; }

    // ========================= AMOUNT =========================
    /// <summary>Payment amount charged.</summary>
    // Type: Decimal (currency amount)
    // Example: 900.00 (for ₹900.00)
    // This is final amount paid (after discount applied)
    public decimal Amount { get; set; }

    // ========================= PAYMENT STATUS =========================
    /// <summary>Current payment status: PENDING, PAID, FAILED, or REFUNDED.</summary>
    // Type: PaymentStatus enum
    // Example values:
    //   PENDING = Payment initiated but not yet completed
    //   PAID = Payment successfully processed
    //   FAILED = Payment attempt failed
    //   REFUNDED = Payment refunded to customer
    // Business Logic: Updated throughout payment lifecycle
    public PaymentStatus Status { get; set; }

    // ========================= MODE OF PAYMENT =========================
    /// <summary>Payment mode used: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET.</summary>
    // Type: PaymentMode enum (1, 2, 3, or 4)
    // Example values:
    //   1 = CASH_ON_DELIVERY
    //   2 = CARD
    //   3 = UPI
    //   4 = WALLET
    // Indicates which payment method was used
    public PaymentMode Mode { get; set; }

    // ========================= TRANSACTION ID =========================
    /// <summary>Transaction ID from the payment gateway (Razorpay/Stripe). Null for CASH_ON_DELIVERY.</summary>
    // Type: String (nullable)
    // Example: "pay_29QQoUBi66xm2f" (Razorpay transaction ID)
    // Source: Payment gateway response after successful transaction
    // Value: Null for CASH_ON_DELIVERY (no online payment)
    // Used for: Payment reconciliation and support queries
    public string? TransactionId { get; set; }

    // ========================= CURRENCY =========================
    /// <summary>Currency code. Typically "INR".</summary>
    // Type: String
    // Example: "INR"
    // ISO 4217 standard currency code
    // Default: "INR"
    public string Currency { get; set; } = "INR";

    // ========================= PAID AT =========================
    /// <summary>Timestamp when payment was marked PAID. Null if not yet paid.</summary>
    // Type: DateTime (nullable)
    // Example: "2026-05-17T14:32:15Z"
    // Value: Null until payment is successfully completed
    // Used for: Payment reconciliation and SLA tracking
    public DateTime? PaidAt { get; set; }

    // ========================= REFUNDED AT =========================
    // RefundedAt: Timestamp when payment was refunded (if applicable)
    // Type: DateTime (nullable)
    // Example: "2026-05-18T10:45:30Z"
    // Value: Only set if payment status = REFUNDED
    // Used for: Refund processing and customer account reconciliation
    public DateTime? RefundedAt { get; set; }
}
// Import for PaymentMode enum (payment method selection)
using QuickBite.Payment.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Payment.Application.DTOs;

// ========================= PROCESS PAYMENT REQUEST DTO =========================
/// <summary>
/// ProcessPaymentRequestDto: Request DTO to process payment for an order
/// Used in POST /api/v1/payments/process endpoint
/// One Payment record created per OrderId; if payment exists, returns existing one
/// 
/// Called by:
/// 1. Frontend (with JWT Authorization header)
/// 2. Order-Service internally (http://localhost:5114/api/v1/payments/process)
///
/// Request body format:
/// {
///   "orderId": "guid-string (required)",
///   "customerId": "guid-string (required)",
///   "amount": decimal (required, must be > 0),
///   "mode": enum-value (1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET, optional default 3),
///   "currency": "string (optional, default 'INR')"
/// }
/// </summary>
public class ProcessPaymentRequestDto
{
    // ========================= ORDER ID =========================
    /// <summary>The order ID for which this payment is being processed. Must be a valid non-empty GUID.</summary>
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Identifies which order this payment is for
    // Business Logic: One payment record per order (duplicate prevented)
    // Required: Yes (cannot be empty)
    public Guid OrderId { get; set; }

    // ========================= CUSTOMER ID =========================
    /// <summary>The customer ID making the payment. Must be a valid non-empty GUID.</summary>
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Identifies customer account for payment
    // Used for wallet deductions and payment history tracking
    // Required: Yes (cannot be empty)
    public Guid CustomerId { get; set; }

    // ========================= AMOUNT =========================
    /// <summary>Payment amount. Must be greater than zero.</summary>
    // Type: Decimal (currency amount)
    // Example: 900.00 (for ₹900.00)
    // Validation: Must be > 0 (no zero or negative amounts)
    // This is final amount after discounts from order
    // Required: Yes (must be provided and > 0)
    public decimal Amount { get; set; }

    // ========================= MODE OF PAYMENT =========================
    /// <summary>
    /// Payment mode: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET.
    /// Default is 3 (UPI). For WALLET, payment is processed from customer's e-wallet.
    /// For COD, no gateway charge is made.
    /// </summary>
    // Type: PaymentMode enum (1, 2, 3, or 4)
    // Example values:
    //   1 = CASH_ON_DELIVERY (no online payment, collect from customer)
    //   2 = CARD (credit/debit card via gateway)
    //   3 = UPI (United Payments Interface via gateway)
    //   4 = WALLET (deduct from QuickBite e-wallet balance)
    // Default: UPI
    // Business Logic:
    //   - CASH_ON_DELIVERY: No gateway charge, payment collected at delivery
    //   - CARD/UPI: Calls payment gateway (Razorpay/Stripe integration)
    //   - WALLET: Deducts from customer wallet balance
    // Required: No (optional, defaults to UPI)
    public PaymentMode Mode { get; set; } = PaymentMode.UPI;

    // ========================= CURRENCY =========================
    /// <summary>Currency code (e.g., "INR", "USD"). Default is "INR".</summary>
    // Type: String
    // Example: "INR" (Indian Rupees)
    // ISO 4217 standard currency codes
    // Default: "INR"
    // Business Logic: Used for international payment gateway communication
    // Required: No (optional, defaults to "INR")
    public string Currency { get; set; } = "INR";
}

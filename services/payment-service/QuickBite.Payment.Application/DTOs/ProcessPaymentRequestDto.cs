using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to process a payment for an order.
/// One Payment record will be created per OrderId. If a payment already exists
/// for the OrderId, the existing payment is returned instead of creating a duplicate.
///
/// This endpoint is called by:
/// 1. Frontend (with JWT Authorization header)
/// 2. Order-Service internally (http://localhost:5114/api/v1/payments/process)
///
/// Expected request body format:
/// {
///   "orderId": "guid-string",
///   "customerId": "guid-string",
///   "amount": decimal (must be > 0),
///   "mode": enum-value (1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET, optional default 3),
///   "currency": "string" (optional, default "INR")
/// }
/// </summary>
public class ProcessPaymentRequestDto
{
    /// <summary>The order ID for which this payment is being processed. Must be a valid non-empty GUID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>The customer ID making the payment. Must be a valid non-empty GUID.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Payment amount. Must be greater than zero.</summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment mode: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET.
    /// Default is 3 (UPI). For WALLET, payment is processed from customer's e-wallet.
    /// For COD, no gateway charge is made.
    /// </summary>
    public PaymentMode Mode { get; set; } = PaymentMode.UPI;

    /// <summary>Currency code (e.g., "INR", "USD"). Default is "INR".</summary>
    public string Currency { get; set; } = "INR";
}

using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.Interfaces;

/// <summary>
/// Abstraction over an upstream payment SDK (Razorpay / Stripe).
/// The default in-process implementation is a no-op stub; a real
/// integration would call the gateway's REST API and return its
/// transaction id.
/// </summary>
public interface IPaymentGatewayClient
{
    /// <summary>Charge the customer. Returns (success, transactionId).</summary>
    Task<(bool Success, string? TransactionId)> ChargeAsync(Guid orderId, decimal amount, string currency, PaymentMode mode);

    /// <summary>Refund a previously captured transaction. Returns true on success.</summary>
    Task<bool> RefundAsync(string transactionId, decimal amount, string currency);
}

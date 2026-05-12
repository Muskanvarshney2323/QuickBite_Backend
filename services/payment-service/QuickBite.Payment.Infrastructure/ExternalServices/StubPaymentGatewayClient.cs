using Microsoft.Extensions.Logging;
using QuickBite.Payment.Application.Interfaces;
using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Infrastructure.ExternalServices;

/// <summary>
/// In-process stub for the upstream payment gateway SDK (Razorpay / Stripe).
/// Always reports success and returns a synthetic transaction id.
/// Replace with a real SDK-backed implementation when the gateway is wired in.
/// </summary>
public class StubPaymentGatewayClient : IPaymentGatewayClient
{
    private readonly ILogger<StubPaymentGatewayClient> _logger;

    public StubPaymentGatewayClient(ILogger<StubPaymentGatewayClient> logger)
    {
        _logger = logger;
    }

    public Task<(bool Success, string? TransactionId)> ChargeAsync(Guid orderId, decimal amount, string currency, PaymentMode mode)
    {
        var txn = $"STUB-{Guid.NewGuid():N}";
        _logger.LogInformation("[StubGateway] Charging {Amount} {Currency} via {Mode} for order {OrderId} -> txn {Txn}",
            amount, currency, mode, orderId, txn);
        return Task.FromResult<(bool, string?)>((true, txn));
    }

    public Task<bool> RefundAsync(string transactionId, decimal amount, string currency)
    {
        _logger.LogInformation("[StubGateway] Refunding {Amount} {Currency} for txn {Txn}",
            amount, currency, transactionId);
        return Task.FromResult(true);
    }
}

using Microsoft.Extensions.Logging;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Infrastructure.ExternalServices;

/// <summary>
/// In-process stub for Payment-Service. Logs the intent and reports success.
/// Replace with an HTTP-backed implementation when Payment-Service is wired in.
/// </summary>
public class StubPaymentGateway : IPaymentGateway
{
    private readonly ILogger<StubPaymentGateway> _logger;

    public StubPaymentGateway(ILogger<StubPaymentGateway> logger)
    {
        _logger = logger;
    }

    public Task<bool> ProcessPaymentAsync(Guid orderId, decimal amount, PaymentMode mode)
    {
        _logger.LogInformation("[StubPaymentGateway] Processing payment for order {OrderId}: amount={Amount} mode={Mode}",
            orderId, amount, mode);
        return Task.FromResult(true);
    }

    public Task<bool> TriggerRefundAsync(Guid orderId, decimal amount, PaymentMode mode)
    {
        _logger.LogInformation("[StubPaymentGateway] Triggering refund for order {OrderId}: amount={Amount} mode={Mode}",
            orderId, amount, mode);
        return Task.FromResult(true);
    }
}

/// <summary>
/// In-process stub for Delivery-Service. Returns a fresh GUID as the "assigned" agent.
/// Replace with an HTTP-backed implementation when Delivery-Service is wired in.
/// </summary>
public class StubDeliveryDispatcher : IDeliveryDispatcher
{
    private readonly ILogger<StubDeliveryDispatcher> _logger;

    public StubDeliveryDispatcher(ILogger<StubDeliveryDispatcher> logger)
    {
        _logger = logger;
    }

    public Task<Guid?> AssignAgentAsync(Guid orderId, Guid restaurantId, string deliveryAddress)
    {
        var assigned = Guid.NewGuid();
        _logger.LogInformation("[StubDeliveryDispatcher] Assigning agent {AgentId} to order {OrderId} (restaurant {RestaurantId})",
            assigned, orderId, restaurantId);
        return Task.FromResult<Guid?>(assigned);
    }
}

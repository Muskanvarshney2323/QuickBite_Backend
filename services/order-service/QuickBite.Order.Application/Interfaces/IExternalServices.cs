namespace QuickBite.Order.Application.Interfaces;

/// <summary>
/// Abstraction over Payment-Service. The default in-process implementation is a no-op
/// stub; in a fully wired-up deployment this would be backed by an HTTP client
/// pointing at the real Payment-Service.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>Process payment for an order. Returns true on success.</summary>
    Task<bool> ProcessPaymentAsync(Guid orderId, Guid customerId, decimal amount, Domain.Enums.PaymentMode mode);

    /// <summary>Trigger a refund for a previously paid order. Returns true on success.</summary>
    Task<bool> TriggerRefundAsync(Guid orderId, decimal amount, Domain.Enums.PaymentMode mode);
}

/// <summary>
/// Abstraction over Delivery-Service. The default in-process implementation is a no-op
/// stub; in a fully wired-up deployment this would be backed by an HTTP client
/// pointing at the real Delivery-Service.
/// </summary>
public interface IDeliveryDispatcher
{
    /// <summary>Ask the Delivery-Service to assign an agent. Returns the agent id, or null if none available.</summary>
    Task<Guid?> AssignAgentAsync(Guid orderId, Guid restaurantId, string deliveryAddress);
}

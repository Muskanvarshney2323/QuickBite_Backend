using QuickBite.Order.Domain.Entities;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.Interfaces;

/// <summary>
/// Repository contract for Order persistence.
/// Method names mirror the Order-Service class diagram spec:
/// FindByOrderId, FindByCustomerId, FindByRestaurantId, FindByOrderStatus,
/// FindByDeliveryAgentId, FindByOrderDateBetween, CountByRestaurantId.
/// </summary>
public interface IOrderRepository
{
    Task<Domain.Entities.Order?> FindByOrderIdAsync(Guid orderId);

    Task<IReadOnlyList<Domain.Entities.Order>> FindByCustomerIdAsync(Guid customerId);

    Task<IReadOnlyList<Domain.Entities.Order>> FindByRestaurantIdAsync(Guid restaurantId);

    Task<IReadOnlyList<Domain.Entities.Order>> FindByOrderStatusAsync(OrderStatus status);

    Task<IReadOnlyList<Domain.Entities.Order>> FindByDeliveryAgentIdAsync(Guid deliveryAgentId);

    Task<IReadOnlyList<Domain.Entities.Order>> FindByOrderDateBetweenAsync(DateTime fromUtc, DateTime toUtc);

    Task<int> CountByRestaurantIdAsync(Guid restaurantId);

    /// <summary>Persist a new order (with its items).</summary>
    Task AddOrderAsync(Domain.Entities.Order order);

    /// <summary>Mark an order as updated.</summary>
    void UpdateOrder(Domain.Entities.Order order);

    /// <summary>Commit pending changes to the database.</summary>
    Task SaveChangesAsync();
}

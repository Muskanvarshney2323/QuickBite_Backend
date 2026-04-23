namespace QuickBite.Order.Domain.Enums;

/// <summary>
/// Lifecycle status of an order, exactly as listed in the Order-Service spec.
/// </summary>
public enum OrderStatus
{
    PLACED = 0,
    CONFIRMED = 1,
    PREPARING = 2,
    PICKED_UP = 3,
    DELIVERED = 4,
    CANCELLED = 5
}

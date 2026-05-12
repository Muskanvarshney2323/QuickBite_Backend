namespace QuickBite.Order.Domain.Enums;

/// <summary>
/// Lifecycle status of an order.
/// </summary>
public enum OrderStatus
{
    PLACED = 0,
    CONFIRMED = 1,
    PREPARING = 2,
    PICKED_UP = 3,
    OUT_FOR_DELIVERY = 4,
    DELIVERED = 5,
    CANCELLED = 6
}
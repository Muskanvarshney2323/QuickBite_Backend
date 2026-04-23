using QuickBite.Order.Application.DTOs;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.Interfaces;

/// <summary>
/// Service contract for order business logic.
/// Mirrors the Order-Service class-diagram methods:
/// PlaceOrder, GetOrderById, GetOrdersByCustomer, GetOrdersByRestaurant,
/// GetActiveOrders, UpdateStatus, AssignDeliveryAgent, CancelOrder,
/// ReorderFromHistory, GetOrderCount.
/// </summary>
public interface IOrderService
{
    /// <summary>Place a new order from a snapshot of cart items.</summary>
    Task<OrderResponseDto> PlaceOrderAsync(PlaceOrderRequestDto request);

    /// <summary>Fetch a single order by id.</summary>
    Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId);

    /// <summary>All orders placed by a customer.</summary>
    Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerAsync(Guid customerId);

    /// <summary>All orders for a restaurant.</summary>
    Task<IReadOnlyList<OrderResponseDto>> GetOrdersByRestaurantAsync(Guid restaurantId);

    /// <summary>Active orders (PLACED / CONFIRMED / PREPARING / PICKED_UP).</summary>
    Task<IReadOnlyList<OrderResponseDto>> GetActiveOrdersAsync();

    /// <summary>Update an order's lifecycle status.</summary>
    Task<OrderResponseDto?> UpdateStatusAsync(Guid orderId, UpdateStatusRequestDto request);

    /// <summary>Assign a delivery agent to an order.</summary>
    Task<OrderResponseDto?> AssignDeliveryAgentAsync(Guid orderId, AssignDeliveryAgentRequestDto request);

    /// <summary>Cancel an order. Triggers a refund for prepaid orders.</summary>
    Task<OrderResponseDto?> CancelOrderAsync(Guid orderId, CancelOrderRequestDto request);

    /// <summary>Place a new order using the items/restaurant of a previous order.</summary>
    Task<OrderResponseDto?> ReorderFromHistoryAsync(ReorderRequestDto request);

    /// <summary>Total number of orders for a restaurant.</summary>
    Task<int> GetOrderCountAsync(Guid restaurantId);
}

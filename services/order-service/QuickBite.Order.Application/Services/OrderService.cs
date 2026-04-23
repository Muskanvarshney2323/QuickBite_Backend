using QuickBite.Order.Application.DTOs;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Domain.Entities;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.Services;

/// <summary>
/// Central orchestration service for orders.
/// Converts a confirmed cart snapshot into an Order, processes payment,
/// asks for a delivery agent, and walks the order through its lifecycle.
/// Cancellation triggers a refund for prepaid orders. Reorder copies a
/// previous order into a fresh placement.
/// </summary>
public class OrderService : IOrderService
{
    private const int DefaultEstimatedMinutes = 45;

    // Statuses considered "active" for live tracking dashboards.
    private static readonly OrderStatus[] ActiveStatuses =
    {
        OrderStatus.PLACED,
        OrderStatus.CONFIRMED,
        OrderStatus.PREPARING,
        OrderStatus.PICKED_UP
    };

    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IDeliveryDispatcher _deliveryDispatcher;

    public OrderService(
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        IDeliveryDispatcher deliveryDispatcher)
    {
        _orderRepository = orderRepository;
        _paymentGateway = paymentGateway;
        _deliveryDispatcher = deliveryDispatcher;
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto> PlaceOrderAsync(PlaceOrderRequestDto request)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new InvalidOperationException("Cannot place an order with no items.");

        if (request.Discount < 0)
            throw new InvalidOperationException("Discount cannot be negative.");

        var totalAmount = request.Items.Sum(i => i.Price * i.Quantity);
        var discount = Math.Min(request.Discount, totalAmount);
        var finalAmount = totalAmount - discount;

        var estimatedMinutes = request.EstimatedMinutes ?? DefaultEstimatedMinutes;

        var order = new Domain.Entities.Order
        {
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            TotalAmount = totalAmount,
            Discount = discount,
            FinalAmount = finalAmount,
            ModeOfPayment = request.ModeOfPayment,
            OrderStatus = OrderStatus.PLACED,
            OrderDate = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddMinutes(estimatedMinutes),
            DeliveryAddress = request.DeliveryAddress,
            SpecialInstructions = request.SpecialInstructions
        };

        foreach (var item in request.Items)
        {
            order.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                MenuItemId = item.MenuItemId,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                Customization = item.Customization
            });
        }

        await _orderRepository.AddOrderAsync(order);
        await _orderRepository.SaveChangesAsync();

        // Process payment via Payment-Service (stubbed by default).
        var paid = await _paymentGateway.ProcessPaymentAsync(order.Id, order.FinalAmount, order.ModeOfPayment);
        if (paid)
        {
            order.OrderStatus = OrderStatus.CONFIRMED;
        }

        // Ask Delivery-Service for an agent (stubbed by default).
        var agentId = await _deliveryDispatcher.AssignAgentAsync(order.Id, order.RestaurantId, order.DeliveryAddress);
        if (agentId.HasValue)
        {
            order.DeliveryAgentId = agentId;
        }

        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.FindByOrderIdAsync(orderId);
        return order is null ? null : MapOrderToResponse(order);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerAsync(Guid customerId)
    {
        var orders = await _orderRepository.FindByCustomerIdAsync(customerId);
        return orders.Select(MapOrderToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OrderResponseDto>> GetOrdersByRestaurantAsync(Guid restaurantId)
    {
        var orders = await _orderRepository.FindByRestaurantIdAsync(restaurantId);
        return orders.Select(MapOrderToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OrderResponseDto>> GetActiveOrdersAsync()
    {
        var collected = new List<Domain.Entities.Order>();
        foreach (var status in ActiveStatuses)
        {
            var batch = await _orderRepository.FindByOrderStatusAsync(status);
            collected.AddRange(batch);
        }

        return collected
            .OrderByDescending(o => o.OrderDate)
            .Select(MapOrderToResponse)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto?> UpdateStatusAsync(Guid orderId, UpdateStatusRequestDto request)
    {
        var order = await _orderRepository.FindByOrderIdAsync(orderId);
        if (order is null) return null;

        if (order.OrderStatus == OrderStatus.CANCELLED)
            throw new InvalidOperationException("Cannot update status of a cancelled order.");

        if (order.OrderStatus == OrderStatus.DELIVERED)
            throw new InvalidOperationException("Cannot update status of a delivered order.");

        // Use the domain method declared on the spec class diagram.
        order.UpdateStatus(request.NewStatus.ToString());

        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto?> AssignDeliveryAgentAsync(Guid orderId, AssignDeliveryAgentRequestDto request)
    {
        var order = await _orderRepository.FindByOrderIdAsync(orderId);
        if (order is null) return null;

        if (order.OrderStatus == OrderStatus.CANCELLED || order.OrderStatus == OrderStatus.DELIVERED)
            throw new InvalidOperationException("Cannot assign an agent to a cancelled or delivered order.");

        if (request.DeliveryAgentId == Guid.Empty)
            throw new InvalidOperationException("DeliveryAgentId is required.");

        order.DeliveryAgentId = request.DeliveryAgentId;
        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto?> CancelOrderAsync(Guid orderId, CancelOrderRequestDto request)
    {
        var order = await _orderRepository.FindByOrderIdAsync(orderId);
        if (order is null) return null;

        if (order.OrderStatus == OrderStatus.DELIVERED)
            throw new InvalidOperationException("Cannot cancel an order that has already been delivered.");

        if (order.OrderStatus == OrderStatus.CANCELLED)
            throw new InvalidOperationException("Order is already cancelled.");

        // Trigger refund only for prepaid orders.
        if (order.ModeOfPayment != PaymentMode.CASH_ON_DELIVERY && order.FinalAmount > 0)
        {
            await _paymentGateway.TriggerRefundAsync(order.Id, order.FinalAmount, order.ModeOfPayment);
        }

        order.OrderStatus = OrderStatus.CANCELLED;
        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        return MapOrderToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponseDto?> ReorderFromHistoryAsync(ReorderRequestDto request)
    {
        var previous = await _orderRepository.FindByOrderIdAsync(request.PreviousOrderId);
        if (previous is null) return null;

        if (previous.CustomerId != request.CustomerId)
            throw new InvalidOperationException("The previous order does not belong to this customer.");

        if (previous.OrderItems.Count == 0)
            throw new InvalidOperationException("The previous order has no items to reorder.");

        var placement = new PlaceOrderRequestDto
        {
            CustomerId = previous.CustomerId,
            RestaurantId = previous.RestaurantId,
            DeliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress)
                ? previous.DeliveryAddress
                : request.DeliveryAddress!,
            ModeOfPayment = request.ModeOfPayment ?? previous.ModeOfPayment,
            SpecialInstructions = previous.SpecialInstructions,
            Discount = 0m, // promo codes are not carried over to the reorder
            Items = previous.OrderItems.Select(i => new PlaceOrderItemDto
            {
                MenuItemId = i.MenuItemId,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity,
                Customization = i.Customization
            }).ToList()
        };

        return await PlaceOrderAsync(placement);
    }

    /// <inheritdoc />
    public async Task<int> GetOrderCountAsync(Guid restaurantId) =>
        await _orderRepository.CountByRestaurantIdAsync(restaurantId);

    private static OrderResponseDto MapOrderToResponse(Domain.Entities.Order order)
    {
        var items = order.OrderItems.Select(i => new OrderItemResponseDto
        {
            OrderItemId = i.Id,
            MenuItemId = i.MenuItemId,
            Name = i.Name,
            Price = i.Price,
            Quantity = i.Quantity,
            Customization = i.Customization,
            LineTotal = i.Price * i.Quantity
        }).ToList();

        return new OrderResponseDto
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            RestaurantId = order.RestaurantId,
            DeliveryAgentId = order.DeliveryAgentId,
            TotalAmount = order.TotalAmount,
            Discount = order.Discount,
            FinalAmount = order.FinalAmount,
            ModeOfPayment = order.ModeOfPayment,
            OrderStatus = order.OrderStatus,
            OrderDate = order.OrderDate,
            EstimatedDelivery = order.EstimatedDelivery,
            DeliveryAddress = order.DeliveryAddress,
            SpecialInstructions = order.SpecialInstructions,
            Items = items
        };
    }
}

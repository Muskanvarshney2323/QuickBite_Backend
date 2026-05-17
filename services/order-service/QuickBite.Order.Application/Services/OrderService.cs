// Used for logging operations and errors
using Microsoft.Extensions.Logging;

// DTO (Data Transfer Object) classes for request/response
using QuickBite.Order.Application.DTOs;

// Service interface definitions
using QuickBite.Order.Application.Interfaces;

// Domain entity classes
using QuickBite.Order.Domain.Entities;

// Enum types (order statuses, payment modes)
using QuickBite.Order.Domain.Enums;

// Namespace for service classes
namespace QuickBite.Order.Application.Services;

// ========================= SUMMARY =========================
/// <summary>
/// OrderService: Central orchestration service for order management
/// Complex business logic including:
/// - Order placement with cart snapshot conversion
/// - Payment processing integration (calls Payment Service)
/// - Delivery agent assignment (calls Delivery Agent Service)
/// - Order status lifecycle management (PLACED → CONFIRMED → PREPARING → PICKED_UP → OUT_FOR_DELIVERY → DELIVERED)
/// - Order cancellation with refunds for prepaid orders
/// - Reorder functionality (copies previous order into new placement)
/// </summary>
public class OrderService : IOrderService
{
    // Default estimated delivery time in minutes if not specified
    private const int DefaultEstimatedMinutes = 45;

    // ========================= ACTIVE STATUSES =========================
    // Statuses considered "active" for live tracking dashboards
    // These are orders in progress that require real-time updates
    private static readonly OrderStatus[] ActiveStatuses =
    {
        // Order placed but payment not yet processed
        OrderStatus.PLACED,

        // Order placed and payment confirmed
        OrderStatus.CONFIRMED,

        // Restaurant is preparing the order
        OrderStatus.PREPARING,

        // Order picked up from restaurant
        OrderStatus.PICKED_UP,

        // Delivery agent is transporting to customer
        OrderStatus.OUT_FOR_DELIVERY
    };

    // Repository for accessing Order data from database
    private readonly IOrderRepository _orderRepository;

    // Gateway for payment processing (calls Payment Service)
    private readonly IPaymentGateway _paymentGateway;

    // Dispatcher for delivery agent assignment (calls Delivery Agent Service)
    private readonly IDeliveryDispatcher _deliveryDispatcher;

    // Logger for tracking order operations
    private readonly ILogger<OrderService> _logger;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    public OrderService(
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        IDeliveryDispatcher deliveryDispatcher,
        ILogger<OrderService> logger)
    {
        // Store repository reference
        _orderRepository = orderRepository;

        // Store payment gateway reference
        _paymentGateway = paymentGateway;

        // Store delivery dispatcher reference
        _deliveryDispatcher = deliveryDispatcher;

        // Store logger reference
        _logger = logger;
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

        _logger.LogInformation("Creating order for customer {CustomerId} at restaurant {RestaurantId}. " +
            "Items={ItemCount}, TotalAmount={TotalAmount}, Discount={Discount}, FinalAmount={FinalAmount}, PaymentMode={Mode}",
            request.CustomerId, request.RestaurantId, request.Items.Count, totalAmount, discount, finalAmount, request.ModeOfPayment);

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

        // Save order to database first with PLACED status.
        await _orderRepository.AddOrderAsync(order);
        await _orderRepository.SaveChangesAsync();
        _logger.LogInformation("Order {OrderId} created successfully with status PLACED", order.Id);

        try
        {
            // Process payment via Payment-Service.
            // For CASH_ON_DELIVERY, payment gateway returns true without hitting the payment service.
            // For other modes, order-service calls payment-service HTTP endpoint.
            _logger.LogInformation("Starting payment processing for order {OrderId}", order.Id);
            var paid = await _paymentGateway.ProcessPaymentAsync(order.Id, order.CustomerId, order.FinalAmount, order.ModeOfPayment);

            if (paid)
            {
                order.OrderStatus = OrderStatus.CONFIRMED;
                _logger.LogInformation("Payment successful for order {OrderId}. Order status changed to CONFIRMED", order.Id);
            }
            else
            {
                _logger.LogWarning("Payment processing returned false for order {OrderId}. Order remains in PLACED status", order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while processing payment for order {OrderId}. Order remains in PLACED status", order.Id);
            // Order remains in PLACED status; payment can be retried
        }

        try
        {
            // Ask Delivery-Service for an agent assignment.
            _logger.LogInformation("Requesting delivery agent assignment for order {OrderId}", order.Id);
            var agentId = await _deliveryDispatcher.AssignAgentAsync(order.Id, order.RestaurantId, order.DeliveryAddress);

            if (agentId.HasValue)
            {
                order.DeliveryAgentId = agentId;
                _logger.LogInformation("Delivery agent {AgentId} assigned to order {OrderId}", agentId, order.Id);
            }
            else
            {
                _logger.LogWarning("No delivery agent available for order {OrderId}", order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while assigning delivery agent to order {OrderId}", order.Id);
            // Order continues without agent assignment; can be assigned later
        }

        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} finalized with status {Status}", order.Id, order.OrderStatus);

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
        var oldStatus = order.OrderStatus;
        order.UpdateStatus(request.NewStatus.ToString());

        _logger.LogInformation("Order {OrderId} status changed from {OldStatus} to {NewStatus}",
            orderId, oldStatus, order.OrderStatus);

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

        _logger.LogInformation("Assigning delivery agent {AgentId} to order {OrderId}",
            request.DeliveryAgentId, orderId);

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

        _logger.LogInformation("Cancelling order {OrderId}. Current status: {Status}, PaymentMode: {PaymentMode}, Amount: {Amount}",
            orderId, order.OrderStatus, order.ModeOfPayment, order.FinalAmount);

        // Trigger refund only for prepaid orders.
        if (order.ModeOfPayment != PaymentMode.CASH_ON_DELIVERY && order.FinalAmount > 0)
        {
            try
            {
                _logger.LogInformation("Triggering refund for order {OrderId} (amount: {Amount})",
                    orderId, order.FinalAmount);

                var refundSuccess = await _paymentGateway.TriggerRefundAsync(order.Id, order.FinalAmount, order.ModeOfPayment);

                if (refundSuccess)
                {
                    _logger.LogInformation("Refund completed successfully for order {OrderId}", orderId);
                }
                else
                {
                    _logger.LogWarning("Refund failed for order {OrderId}. Order will still be cancelled but customer may need manual refund", orderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while triggering refund for order {OrderId}", orderId);
                // Order will still be cancelled; refund error is logged for manual intervention
            }
        }
        else
        {
            _logger.LogInformation("Skipping refund for order {OrderId}: PaymentMode={PaymentMode}", orderId, order.ModeOfPayment);
        }

        order.OrderStatus = OrderStatus.CANCELLED;
        _orderRepository.UpdateOrder(order);
        await _orderRepository.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);

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

        _logger.LogInformation("Creating new order from previous order {PreviousOrderId} for customer {CustomerId}",
            request.PreviousOrderId, request.CustomerId);

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

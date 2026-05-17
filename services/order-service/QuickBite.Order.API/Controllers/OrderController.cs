// Used for authorization features like [Authorize] attribute
using Microsoft.AspNetCore.Authorization;

// Used for Web API controller features, routing, IActionResult, etc.
using Microsoft.AspNetCore.Mvc;

// DTO (Data Transfer Object) classes for request/response
using QuickBite.Order.Application.DTOs;

// Service interface for business logic
using QuickBite.Order.Application.Interfaces;

// Used for Swagger/OpenAPI documentation
using Swashbuckle.AspNetCore.Annotations;

// Used for database context access
using Microsoft.EntityFrameworkCore;

// Enum types for order status and payment modes
using QuickBite.Order.Domain.Enums;

// Database context for orders
using QuickBite.Order.Infrastructure.Data;

// Namespace for this controller
namespace QuickBite.Order.API.Controllers;

// ========================= ORDER CONTROLLER SUMMARY =========================
/// <summary>
/// OrderController: Exposes HTTP endpoints for order operations
/// Endpoints: place order, get order, get by customer/restaurant, get active orders,
/// update status, assign delivery agent, cancel order, reorder
/// </summary>

// ========================= ATTRIBUTES =========================

// Mark this class as an API Controller (enables automatic model validation)
[ApiController]

// Base route for all endpoints in this controller
// Example: POST /api/v1/orders
[Route("api/v1/orders")]

// Requires JWT authentication token for all endpoints
[Authorize]

// Swagger documentation tag
[SwaggerTag("Order operations")]
public class OrderController : ControllerBase
{
    // Service object that contains all order business logic
    private readonly IOrderService _orderService;

    // Database context for direct database access (used for reporting)
    private readonly OrderDbContext _context;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    public OrderController(
        IOrderService orderService,
        OrderDbContext context)
    {
        // Store service reference for use in endpoint methods
        _orderService = orderService;

        // Store context reference
        _context = context;
    }

    // Place order
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequestDto request)
    {
        var result = await _orderService.PlaceOrderAsync(request);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { orderId = result.OrderId },
            result);
    }

    // Get single order
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(orderId);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

    // Get customer orders
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetOrdersByCustomer(Guid customerId)
    {
        var result = await _orderService.GetOrdersByCustomerAsync(customerId);

        return Ok(result);
    }

    // Get restaurant orders
    [HttpGet("restaurant/{restaurantId:guid}")]
    public async Task<IActionResult> GetOrdersByRestaurant(Guid restaurantId)
    {
        var result = await _orderService.GetOrdersByRestaurantAsync(restaurantId);

        return Ok(result);
    }

    // Get active orders
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveOrders()
    {
        var result = await _orderService.GetActiveOrdersAsync();

        return Ok(result);
    }

    // Update status
    [HttpPut("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid orderId,
        [FromBody] UpdateStatusRequestDto request)
    {
        var result = await _orderService.UpdateStatusAsync(orderId, request);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

    // Assign delivery agent
    [HttpPut("{orderId:guid}/assignAgent")]
    public async Task<IActionResult> AssignAgent(
        Guid orderId,
        [FromBody] AssignDeliveryAgentRequestDto request)
    {
        var result = await _orderService.AssignDeliveryAgentAsync(orderId, request);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

    // Cancel order
    [HttpPut("{orderId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid orderId,
        [FromBody] CancelOrderRequestDto request)
    {
        var result = await _orderService.CancelOrderAsync(orderId, request);

        if (result == null)
            return NotFound("Order not found");

        return Ok(result);
    }

    // Reorder
    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder(
        [FromBody] ReorderRequestDto request)
    {
        var result = await _orderService.ReorderFromHistoryAsync(request);

        if (result == null)
            return NotFound("Previous order not found");

        return Ok(result);
    }

    // Restaurant order count
    [HttpGet("count/{restaurantId:guid}")]
    public async Task<IActionResult> GetOrderCount(Guid restaurantId)
    {
        var count = await _orderService.GetOrderCountAsync(restaurantId);

        return Ok(new
        {
            restaurantId,
            count
        });
    }

    // DELIVERY HISTORY API
    [HttpGet("agent/{agentId:guid}/history")]
    public async Task<IActionResult> GetDeliveryHistory(Guid agentId)
    {
        var orders = await _context.Orders
            .Where(o =>
                o.DeliveryAgentId == agentId &&
                o.OrderStatus == OrderStatus.DELIVERED)
            .OrderByDescending(o => o.UpdatedAt)
            .ToListAsync();

        return Ok(orders);
    }
}
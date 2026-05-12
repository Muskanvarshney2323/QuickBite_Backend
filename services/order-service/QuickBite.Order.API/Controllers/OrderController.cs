using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Order.Application.DTOs;
using QuickBite.Order.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
using QuickBite.Order.Domain.Enums;
using QuickBite.Order.Infrastructure.Data;

namespace QuickBite.Order.API.Controllers;

/// <summary>
/// Exposes /api/v1/orders endpoints for placement, retrieval,
/// status management, agent assignment, cancellation, and reorder.
/// </summary>
[ApiController]
[Route("api/v1/orders")]
[Authorize]
[SwaggerTag("Order operations")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly OrderDbContext _context;

    public OrderController(
        IOrderService orderService,
        OrderDbContext context)
    {
        _orderService = orderService;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Order.Application.DTOs;
using QuickBite.Order.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Order.API.Controllers;

/// <summary>
/// Exposes /api/v1/orders endpoints for placement, retrieval,
/// status management, agent assignment, cancellation, and reorder.
/// </summary>
[ApiController]
[Route("api/v1/orders")]
[Authorize]
[SwaggerTag("Order operations: place, get (by id/customer/restaurant/active), update status, assign agent, cancel, reorder, count")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>  place a new order.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Place order", Description = "Creates a new order from a snapshot of cart items and orchestrates payment + delivery agent assignment.")]
    [SwaggerResponse(201, "Order placed successfully", typeof(OrderResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequestDto request)
    {
        if (request.CustomerId == Guid.Empty) return BadRequest("CustomerId is required.");
        if (request.RestaurantId == Guid.Empty) return BadRequest("RestaurantId is required.");
        if (string.IsNullOrWhiteSpace(request.DeliveryAddress)) return BadRequest("DeliveryAddress is required.");

        try
        {
            var result = await _orderService.PlaceOrderAsync(request);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = result.OrderId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  fetch a single order.</summary>
    [HttpGet("{orderId:guid}")]
    [SwaggerOperation(Summary = "Get order by id")]
    [SwaggerResponse(200, "Order found", typeof(OrderResponseDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(orderId);
        return result is null ? NotFound("Order not found.") : Ok(result);
    }

    /// <summary>  all orders for a customer.</summary>
    [HttpGet("customer/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get orders by customer")]
    [SwaggerResponse(200, "Orders fetched", typeof(IReadOnlyList<OrderResponseDto>))]
    public async Task<IActionResult> GetOrdersByCustomer(Guid customerId)
    {
        var result = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(result);
    }

    /// <summary>  all orders for a restaurant.</summary>
    [HttpGet("restaurant/{restaurantId:guid}")]
    [SwaggerOperation(Summary = "Get orders by restaurant")]
    [SwaggerResponse(200, "Orders fetched", typeof(IReadOnlyList<OrderResponseDto>))]
    public async Task<IActionResult> GetOrdersByRestaurant(Guid restaurantId)
    {
        var result = await _orderService.GetOrdersByRestaurantAsync(restaurantId);
        return Ok(result);
    }

    /// <summary>  all currently active orders (PLACED/CONFIRMED/PREPARING/PICKED_UP).</summary>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get active orders")]
    [SwaggerResponse(200, "Active orders fetched", typeof(IReadOnlyList<OrderResponseDto>))]
    public async Task<IActionResult> GetActiveOrders()
    {
        var result = await _orderService.GetActiveOrdersAsync();
        return Ok(result);
    }

    /// <summary>  update an order's lifecycle status.</summary>
    [HttpPut("{orderId:guid}/status")]
    [SwaggerOperation(Summary = "Update status")]
    [SwaggerResponse(200, "Status updated", typeof(OrderResponseDto))]
    [SwaggerResponse(400, "Invalid status transition")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateStatusRequestDto request)
    {
        try
        {
            var result = await _orderService.UpdateStatusAsync(orderId, request);
            return result is null ? NotFound("Order not found.") : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  assign a delivery agent.</summary>
    [HttpPut("{orderId:guid}/assignAgent")]
    [SwaggerOperation(Summary = "Assign delivery agent")]
    [SwaggerResponse(200, "Agent assigned", typeof(OrderResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> AssignAgent(Guid orderId, [FromBody] AssignDeliveryAgentRequestDto request)
    {
        try
        {
            var result = await _orderService.AssignDeliveryAgentAsync(orderId, request);
            return result is null ? NotFound("Order not found.") : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  cancel an order (refund triggered for prepaid orders).</summary>
    [HttpPut("{orderId:guid}/cancel")]
    [SwaggerOperation(Summary = "Cancel order", Description = "Cancels the order. Refund is triggered automatically when the order was prepaid.")]
    [SwaggerResponse(200, "Order cancelled", typeof(OrderResponseDto))]
    [SwaggerResponse(400, "Cannot cancel in current state")]
    [SwaggerResponse(404, "Order not found")]
    public async Task<IActionResult> Cancel(Guid orderId, [FromBody] CancelOrderRequestDto request)
    {
        try
        {
            var result = await _orderService.CancelOrderAsync(orderId, request);
            return result is null ? NotFound("Order not found.") : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>POST /api/v1/orders/reorder - place a new order from a previous one.</summary>
    [HttpPost("reorder")]
    [SwaggerOperation(Summary = "Reorder from history")]
    [SwaggerResponse(201, "Reorder placed", typeof(OrderResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "Previous order not found")]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequestDto request)
    {
        if (request.CustomerId == Guid.Empty) return BadRequest("CustomerId is required.");
        if (request.PreviousOrderId == Guid.Empty) return BadRequest("PreviousOrderId is required.");

        try
        {
            var result = await _orderService.ReorderFromHistoryAsync(request);
            if (result is null) return NotFound("Previous order not found.");
            return CreatedAtAction(nameof(GetOrderById), new { orderId = result.OrderId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>GET /api/v1/orders/count/{restaurantId} - total order count for a restaurant.</summary>
    [HttpGet("count/{restaurantId:guid}")]
    [SwaggerOperation(Summary = "Get order count for a restaurant")]
    [SwaggerResponse(200, "Count returned")]
    public async Task<IActionResult> GetOrderCount(Guid restaurantId)
    {
        var count = await _orderService.GetOrderCountAsync(restaurantId);
        return Ok(new { restaurantId, count });
    }
}

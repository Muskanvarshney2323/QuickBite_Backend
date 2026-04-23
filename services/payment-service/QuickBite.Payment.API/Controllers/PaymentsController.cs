using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Payment.Application.DTOs;
using QuickBite.Payment.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Payment.API.Controllers;

/// <summary>
/// Exposes /api/v1/payments endpoints for processing payments,
/// retrieving payment records, refunds, and status updates.
/// </summary>
[ApiController]
[Route("api/v1/payments")]
[Authorize]
[SwaggerTag("Payment operations: process, get by order, get by customer, refund, update status")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>POST /api/v1/payments/process - process a payment for an order.</summary>
    [HttpPost("process")]
    [SwaggerOperation(Summary = "Process payment", Description = "Charges the customer via the configured gateway and creates a Payment record.")]
    [SwaggerResponse(201, "Payment processed", typeof(PaymentResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> Process([FromBody] ProcessPaymentRequestDto request)
    {
        try
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            return CreatedAtAction(nameof(GetByOrder), new { orderId = result.OrderId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>GET /api/v1/payments/byOrder/{orderId} - fetch the payment for an order.</summary>
    [HttpGet("byOrder/{orderId:guid}")]
    [SwaggerOperation(Summary = "Get payment by order")]
    [SwaggerResponse(200, "Payment found", typeof(PaymentResponseDto))]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await _paymentService.GetByOrderAsync(orderId);
        return result is null ? NotFound("Payment not found.") : Ok(result);
    }

    /// <summary>GET /api/v1/payments/byCustomer/{customerId} - all payments by a customer.</summary>
    [HttpGet("byCustomer/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get payments by customer")]
    [SwaggerResponse(200, "Payments fetched", typeof(IReadOnlyList<PaymentResponseDto>))]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var result = await _paymentService.GetByCustomerAsync(customerId);
        return Ok(result);
    }

    /// <summary>POST /api/v1/payments/{paymentId}/refund - refund a captured payment.</summary>
    [HttpPost("{paymentId:guid}/refund")]
    [SwaggerOperation(Summary = "Refund payment", Description = "Refunds a previously PAID payment. Wallet refunds credit the wallet; gateway refunds hit the SDK.")]
    [SwaggerResponse(200, "Payment refunded", typeof(PaymentResponseDto))]
    [SwaggerResponse(400, "Cannot refund in current state")]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<IActionResult> Refund(Guid paymentId, [FromBody] RefundRequestDto request)
    {
        try
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, request);
            return result is null ? NotFound("Payment not found.") : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>PUT /api/v1/payments/{paymentId}/status - update a payment's status.</summary>
    [HttpPut("{paymentId:guid}/status")]
    [SwaggerOperation(Summary = "Update payment status")]
    [SwaggerResponse(200, "Status updated", typeof(PaymentResponseDto))]
    [SwaggerResponse(400, "Invalid status")]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<IActionResult> UpdateStatus(Guid paymentId, [FromBody] UpdatePaymentStatusRequestDto request)
    {
        try
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(paymentId, request);
            return result is null ? NotFound("Payment not found.") : Ok(result);
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
}

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

    /// <summary>Process payment for an order (COD, CARD, UPI, or WALLET).</summary>
    [HttpPost("process")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Process payment", Description = "Creates payment record and charges gateway if applicable.")]
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

    /// <summary>Get payment record for a specific order.</summary>
    [HttpGet("byOrder/{orderId:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get payment by order")]
    [SwaggerResponse(200, "Payment found", typeof(PaymentResponseDto))]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await _paymentService.GetByOrderAsync(orderId);
        return result is null ? NotFound("Payment not found.") : Ok(result);
    }

    /// <summary>Get all payments for a customer.</summary>
    [HttpGet("byCustomer/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get payments by customer")]
    [SwaggerResponse(200, "Payments fetched", typeof(IReadOnlyList<PaymentResponseDto>))]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var result = await _paymentService.GetByCustomerAsync(customerId);
        return Ok(result);
    }

    /// <summary>Refund a PAID payment (wallet credit or gateway reversal).</summary>
    [HttpPost("{paymentId:guid}/refund")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refund payment", Description = "Refunds via wallet or gateway based on payment mode.")]
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

    /// <summary>Update payment status (PENDING, PAID, FAILED, REFUNDED).</summary>
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Payment.Application.DTOs;
using QuickBite.Payment.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Payment.API.Controllers;

/// <summary>
/// Exposes /api/v1/wallet endpoints for balance retrieval,
/// top-ups, paying for orders from the wallet, and statement retrieval.
/// </summary>
[ApiController]
[Route("api/v1/wallet")]
[Authorize]
[SwaggerTag("Wallet operations: balance, addToWallet, payFromWallet, statements")]
public class WalletController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public WalletController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary> get the customer's wallet balance.</summary>
    [HttpGet("balance/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get wallet balance", Description = "Returns the customer's spendable wallet balance. Creates a wallet on first call.")]
    [SwaggerResponse(200, "Balance returned", typeof(WalletBalanceResponseDto))]
    public async Task<IActionResult> GetBalance(Guid customerId)
    {
        if (customerId == Guid.Empty) return BadRequest("CustomerId is required.");

        var result = await _paymentService.GetWalletBalanceAsync(customerId);
        return Ok(result);
    }

    /// <summary> top up the customer's wallet.</summary>
    [HttpPost("addToWallet")]
    [SwaggerOperation(Summary = "Top up wallet", Description = "Charges the funding source via the gateway and credits the wallet.")]
    [SwaggerResponse(200, "Wallet topped up", typeof(WalletBalanceResponseDto))]
    [SwaggerResponse(400, "Invalid request or gateway declined")]
    public async Task<IActionResult> AddToWallet([FromBody] AddToWalletRequestDto request)
    {
        try
        {
            var result = await _paymentService.AddToWalletAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  pay for an order from the wallet balance.</summary>
    [HttpPost("payFromWallet")]
    [SwaggerOperation(Summary = "Pay from wallet", Description = "Validates sufficient balance, debits the wallet, and creates a PAID Payment record.")]
    [SwaggerResponse(201, "Payment created", typeof(PaymentResponseDto))]
    [SwaggerResponse(400, "Insufficient balance or invalid request")]
    public async Task<IActionResult> PayFromWallet([FromBody] PayFromWalletRequestDto request)
    {
        try
        {
            var result = await _paymentService.PayFromWalletAsync(request);
            return CreatedAtAction(
                actionName: nameof(PaymentsController.GetByOrder),
                controllerName: "Payments",
                routeValues: new { orderId = result.OrderId },
                value: result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>  get the customer's wallet ledger.</summary>
    [HttpGet("statements/{customerId:guid}")]
    [SwaggerOperation(Summary = "Get wallet statements", Description = "Returns the wallet ledger entries (deposits + debits), newest first.")]
    [SwaggerResponse(200, "Statements returned", typeof(IReadOnlyList<WalletStatementResponseDto>))]
    public async Task<IActionResult> GetStatements(Guid customerId)
    {
        if (customerId == Guid.Empty) return BadRequest("CustomerId is required.");

        var result = await _paymentService.GetWalletStatementsAsync(customerId);
        return Ok(result);
    }
}

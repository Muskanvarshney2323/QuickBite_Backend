using QuickBite.Payment.Application.DTOs;

namespace QuickBite.Payment.Application.Interfaces;

/// <summary>
/// Service contract for payment + wallet business logic.
/// Mirrors the PaymentService class diagram methods:
/// ProcessPayment, GetByOrder, GetByCustomer, RefundPayment,
/// GetWalletBalance, AddToWallet, PayFromWallet,
/// GetWalletStatements, UpdatePaymentStatus.
/// </summary>
public interface IPaymentService
{
    // ----- Payment -----

    /// <summary>Process a payment for an order via the configured gateway.</summary>
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request);

    /// <summary>Fetch the payment record for a given order.</summary>
    Task<PaymentResponseDto?> GetByOrderAsync(Guid orderId);

    /// <summary>All payments made by a customer.</summary>
    Task<IReadOnlyList<PaymentResponseDto>> GetByCustomerAsync(Guid customerId);

    /// <summary>Refund a previously captured payment (e.g. on order cancellation).</summary>
    Task<PaymentResponseDto?> RefundPaymentAsync(Guid paymentId, RefundRequestDto request);

    /// <summary>Update a payment's lifecycle status.</summary>
    Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid paymentId, UpdatePaymentStatusRequestDto request);

    // ----- Wallet -----

    /// <summary>Get the customer's wallet balance (creates a wallet on first call).</summary>
    Task<WalletBalanceResponseDto> GetWalletBalanceAsync(Guid customerId);

    /// <summary>Top up the customer's wallet.</summary>
    Task<WalletBalanceResponseDto> AddToWalletAsync(AddToWalletRequestDto request);

    /// <summary>
    /// Pay for an order out of the customer's wallet balance.
    /// Validates sufficient balance before debiting.
    /// </summary>
    Task<PaymentResponseDto> PayFromWalletAsync(PayFromWalletRequestDto request);

    /// <summary>Get the wallet ledger entries for a customer, newest first.</summary>
    Task<IReadOnlyList<WalletStatementResponseDto>> GetWalletStatementsAsync(Guid customerId);
}

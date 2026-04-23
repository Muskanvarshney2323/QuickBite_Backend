using QuickBite.Payment.Application.DTOs;
using QuickBite.Payment.Application.Interfaces;
using QuickBite.Payment.Domain.Entities;
using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.Services;

/// <summary>
/// Business logic for payments and wallets.
/// Validates sufficient wallet balance before debiting; charges via the
/// configured gateway client (Razorpay / Stripe in production); records
/// every wallet movement as a WalletStatement entry; and triggers refunds
/// when a payment is reversed (e.g. on order cancellation).
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPaymentGatewayClient _gateway;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IWalletRepository walletRepository,
        IPaymentGatewayClient gateway)
    {
        _paymentRepository = paymentRepository;
        _walletRepository = walletRepository;
        _gateway = gateway;
    }

    // ----- Payment -----

    /// <inheritdoc />
    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");
        if (request.OrderId == Guid.Empty)
            throw new InvalidOperationException("OrderId is required.");
        if (request.CustomerId == Guid.Empty)
            throw new InvalidOperationException("CustomerId is required.");

        // One Payment per Order — refuse a duplicate.
        var existing = await _paymentRepository.FindByOrderIdAsync(request.OrderId);
        if (existing is not null)
            throw new InvalidOperationException($"A payment already exists for order {request.OrderId}.");

        // Wallet payments take a different code path — callers should hit PayFromWallet instead.
        if (request.Mode == PaymentMode.WALLET)
            throw new InvalidOperationException("Use PayFromWallet for wallet-mode payments.");

        var payment = new Domain.Entities.Payment
        {
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR" : request.Currency,
            Mode = request.Mode,
            Status = PaymentStatus.PENDING
        };

        await _paymentRepository.AddPaymentAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        // COD = no gateway charge; payment will be captured at delivery.
        if (request.Mode == PaymentMode.COD)
        {
            return MapPaymentToResponse(payment);
        }

        // Charge via the configured gateway (stub in dev, Razorpay/Stripe in prod).
        var (success, txnId) = await _gateway.ChargeAsync(payment.OrderId, payment.Amount, payment.Currency, payment.Mode);
        payment.TransactionId = txnId;

        if (success)
        {
            payment.Status = PaymentStatus.PAID;
            payment.PaidAt = DateTime.UtcNow;
        }
        else
        {
            payment.Status = PaymentStatus.FAILED;
        }

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> GetByOrderAsync(Guid orderId)
    {
        var payment = await _paymentRepository.FindByOrderIdAsync(orderId);
        return payment is null ? null : MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaymentResponseDto>> GetByCustomerAsync(Guid customerId)
    {
        var payments = await _paymentRepository.FindByCustomerIdAsync(customerId);
        return payments.Select(MapPaymentToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> RefundPaymentAsync(Guid paymentId, RefundRequestDto request)
    {
        var payment = await _paymentRepository.FindByIdAsync(paymentId);
        if (payment is null) return null;

        if (payment.Status == PaymentStatus.REFUNDED)
            throw new InvalidOperationException("Payment is already refunded.");
        if (payment.Status != PaymentStatus.PAID)
            throw new InvalidOperationException("Only PAID payments can be refunded.");

        // Wallet refunds: credit the wallet back. Gateway refunds: hit the SDK.
        if (payment.Mode == PaymentMode.WALLET)
        {
            var wallet = await EnsureWalletAsync(payment.CustomerId);
            CreditWallet(wallet, payment.Amount,
                reference: payment.OrderId.ToString(),
                description: $"Refund for order {payment.OrderId}: {request.Reason ?? "cancellation"}");
            _walletRepository.UpdateWallet(wallet);
            await _walletRepository.SaveChangesAsync();
        }
        else if (payment.Mode != PaymentMode.COD)
        {
            // Razorpay / Stripe refund. COD has no upstream charge to refund.
            if (!string.IsNullOrWhiteSpace(payment.TransactionId))
            {
                await _gateway.RefundAsync(payment.TransactionId!, payment.Amount, payment.Currency);
            }
        }

        payment.Status = PaymentStatus.REFUNDED;
        payment.RefundedAt = DateTime.UtcNow;

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid paymentId, UpdatePaymentStatusRequestDto request)
    {
        var payment = await _paymentRepository.FindByIdAsync(paymentId);
        if (payment is null) return null;

        // Use the domain method declared on the spec class diagram.
        payment.UpdatePaymentStatus(request.NewStatus.ToString());

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapPaymentToResponse(payment);
    }

    // ----- Wallet -----

    /// <inheritdoc />
    public async Task<WalletBalanceResponseDto> GetWalletBalanceAsync(Guid customerId)
    {
        var wallet = await EnsureWalletAsync(customerId);
        return new WalletBalanceResponseDto
        {
            WalletId = wallet.Id,
            CustomerId = wallet.CustomerId,
            Balance = wallet.Balance
        };
    }

    /// <inheritdoc />
    public async Task<WalletBalanceResponseDto> AddToWalletAsync(AddToWalletRequestDto request)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Top-up amount must be greater than zero.");
        if (request.CustomerId == Guid.Empty)
            throw new InvalidOperationException("CustomerId is required.");

        var wallet = await EnsureWalletAsync(request.CustomerId);

        // In production the funding charge would also go through the gateway
        // before crediting the wallet. The stub gateway always succeeds.
        if (request.FundingMode != PaymentMode.COD)
        {
            var (success, _) = await _gateway.ChargeAsync(
                orderId: Guid.Empty, // top-ups aren't tied to an order
                amount: request.Amount,
                currency: "INR",
                mode: request.FundingMode);
            if (!success)
                throw new InvalidOperationException("Wallet top-up failed: gateway charge declined.");
        }

        CreditWallet(wallet, request.Amount,
            reference: request.Reference,
            description: $"Top-up via {request.FundingMode}");

        _walletRepository.UpdateWallet(wallet);
        await _walletRepository.SaveChangesAsync();

        return new WalletBalanceResponseDto
        {
            WalletId = wallet.Id,
            CustomerId = wallet.CustomerId,
            Balance = wallet.Balance
        };
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto> PayFromWalletAsync(PayFromWalletRequestDto request)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");
        if (request.OrderId == Guid.Empty)
            throw new InvalidOperationException("OrderId is required.");
        if (request.CustomerId == Guid.Empty)
            throw new InvalidOperationException("CustomerId is required.");

        var existing = await _paymentRepository.FindByOrderIdAsync(request.OrderId);
        if (existing is not null)
            throw new InvalidOperationException($"A payment already exists for order {request.OrderId}.");

        var wallet = await EnsureWalletAsync(request.CustomerId);

        // Validate sufficient balance before debiting.
        if (wallet.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient wallet balance.");

        DebitWallet(wallet, request.Amount,
            reference: request.OrderId.ToString(),
            description: $"Payment for order {request.OrderId}");

        var payment = new Domain.Entities.Payment
        {
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Currency = "INR",
            Mode = PaymentMode.WALLET,
            Status = PaymentStatus.PAID,
            PaidAt = DateTime.UtcNow,
            TransactionId = $"WALLET-{Guid.NewGuid():N}"
        };

        await _paymentRepository.AddPaymentAsync(payment);
        _walletRepository.UpdateWallet(wallet);

        await _paymentRepository.SaveChangesAsync();
        await _walletRepository.SaveChangesAsync();

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatementResponseDto>> GetWalletStatementsAsync(Guid customerId)
    {
        var statements = await _walletRepository.GetStatementsByCustomerIdAsync(customerId);
        return statements.Select(s => new WalletStatementResponseDto
        {
            StatementId = s.Id,
            Type = s.Type,
            Amount = s.Amount,
            BalanceAfter = s.BalanceAfter,
            Reference = s.Reference,
            Description = s.Description,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    // ----- helpers -----

    private async Task<Wallet> EnsureWalletAsync(Guid customerId)
    {
        var wallet = await _walletRepository.FindByCustomerIdAsync(customerId);
        if (wallet is not null) return wallet;

        wallet = new Wallet
        {
            CustomerId = customerId,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _walletRepository.AddWalletAsync(wallet);
        await _walletRepository.SaveChangesAsync();
        return wallet;
    }

    private static void CreditWallet(Wallet wallet, decimal amount, string? reference, string? description)
    {
        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        wallet.WalletStatements.Add(new WalletStatement
        {
            WalletId = wallet.Id,
            Type = WalletStatementType.DEPOSIT,
            Amount = amount,
            BalanceAfter = wallet.Balance,
            Reference = reference,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static void DebitWallet(Wallet wallet, decimal amount, string? reference, string? description)
    {
        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        wallet.WalletStatements.Add(new WalletStatement
        {
            WalletId = wallet.Id,
            Type = WalletStatementType.DEBIT,
            Amount = amount,
            BalanceAfter = wallet.Balance,
            Reference = reference,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static PaymentResponseDto MapPaymentToResponse(Domain.Entities.Payment p) => new()
    {
        PaymentId = p.Id,
        OrderId = p.OrderId,
        CustomerId = p.CustomerId,
        Amount = p.Amount,
        Status = p.Status,
        Mode = p.Mode,
        TransactionId = p.TransactionId,
        Currency = p.Currency,
        PaidAt = p.PaidAt,
        RefundedAt = p.RefundedAt
    };
}

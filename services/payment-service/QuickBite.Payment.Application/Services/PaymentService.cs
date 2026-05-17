// Used for logging operations and errors
using Microsoft.Extensions.Logging;

// DTO (Data Transfer Object) classes for request/response
using QuickBite.Payment.Application.DTOs;

// Service interface definitions
using QuickBite.Payment.Application.Interfaces;

// Domain entity classes
using QuickBite.Payment.Domain.Entities;

// Enum types (payment modes, statuses)
using QuickBite.Payment.Domain.Enums;

// Namespace for service classes
namespace QuickBite.Payment.Application.Services;

// ========================= SUMMARY =========================
/// <summary>
/// PaymentService: Business logic for payment processing and wallet management
/// Features:
/// - Multi-mode payment support (COD, CARD, UPI, WALLET)
/// - Payment gateway integration (Razorpay/Stripe in production)
/// - Wallet balance validation and debit tracking
/// - Refund processing for cancelled orders
/// - Transaction logging and audit trail
/// </summary>
public class PaymentService : IPaymentService
{
    // Repository for accessing Payment data from database
    private readonly IPaymentRepository _paymentRepository;

    // Repository for accessing Wallet data from database
    private readonly IWalletRepository _walletRepository;

    // Client for charging payment gateway (Razorpay, Stripe, etc.)
    private readonly IPaymentGatewayClient _gateway;

    // Logger for tracking payment operations
    private readonly ILogger<PaymentService> _logger;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    public PaymentService(
        IPaymentRepository paymentRepository,
        IWalletRepository walletRepository,
        IPaymentGatewayClient gateway,
        ILogger<PaymentService> logger)
    {
        // Store payment repository reference
        _paymentRepository = paymentRepository;

        // Store wallet repository reference
        _walletRepository = walletRepository;

        // Store payment gateway client reference
        _gateway = gateway;

        // Store logger reference
        _logger = logger;
    }

    // ----- Payment -----

    /// <inheritdoc />
    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentRequestDto request)
    {
        // Validate required fields
        if (request.Amount <= 0)
        {
            _logger.LogWarning("Invalid payment amount: {Amount} for order {OrderId}", request.Amount, request.OrderId);
            throw new InvalidOperationException("Amount must be greater than zero.");
        }

        if (request.OrderId == Guid.Empty)
        {
            _logger.LogWarning("OrderId is empty");
            throw new InvalidOperationException("OrderId is required.");
        }

        if (request.CustomerId == Guid.Empty)
        {
            _logger.LogWarning("CustomerId is empty for order {OrderId}", request.OrderId);
            throw new InvalidOperationException("CustomerId is required.");
        }

        _logger.LogInformation("Processing payment for order {OrderId}. Customer: {CustomerId}, Amount: {Amount}, Mode: {Mode}, Currency: {Currency}",
            request.OrderId, request.CustomerId, request.Amount, request.Mode, request.Currency);

        // Check if payment already exists for this order
        var existing = await _paymentRepository.FindByOrderIdAsync(request.OrderId);
        if (existing is not null)
        {
            _logger.LogInformation("Payment already exists for order {OrderId}. Returning existing payment. PaymentId: {PaymentId}, Status: {Status}",
                request.OrderId, existing.Id, existing.Status);
            return MapPaymentToResponse(existing);
        }

        // Wallet payments use wallet balance internally, but the same /process endpoint
        // can safely handle it for frontend and Order-Service simplicity.
        if (request.Mode == PaymentMode.WALLET)
        {
            _logger.LogInformation("Processing wallet payment for order {OrderId}", request.OrderId);
            return await PayFromWalletAsync(new PayFromWalletRequestDto
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                Amount = request.Amount
            });
        }

        // Create payment record with PENDING status
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
        _logger.LogInformation("Payment record created for order {OrderId}. PaymentId: {PaymentId}, Status: PENDING",
            request.OrderId, payment.Id);

        // COD = no gateway charge; payment will be captured at delivery.
        if (request.Mode == PaymentMode.CASH_ON_DELIVERY)
        {
            _logger.LogInformation("Payment mode is CASH_ON_DELIVERY. Skipping gateway charge for order {OrderId}",
                request.OrderId);
            return MapPaymentToResponse(payment);
        }

        try
        {
            // Charge via the configured gateway (stub in dev, Razorpay/Stripe in prod).
            _logger.LogInformation("Charging payment gateway for order {OrderId}. Amount: {Amount}, Currency: {Currency}, Mode: {Mode}",
                request.OrderId, payment.Amount, payment.Currency, request.Mode);

            var (success, txnId) = await _gateway.ChargeAsync(payment.OrderId, payment.Amount, payment.Currency, payment.Mode);
            payment.TransactionId = txnId;

            if (success)
            {
                payment.Status = PaymentStatus.PAID;
                payment.PaidAt = DateTime.UtcNow;
                _logger.LogInformation("Payment charged successfully for order {OrderId}. TransactionId: {TransactionId}, Status: PAID",
                    request.OrderId, txnId);
            }
            else
            {
                payment.Status = PaymentStatus.FAILED;
                _logger.LogWarning("Payment gateway declined charge for order {OrderId}. Status: FAILED. TransactionId: {TransactionId}",
                    request.OrderId, txnId);
            }
        }
        catch (Exception ex)
        {
            payment.Status = PaymentStatus.FAILED;
            _logger.LogError(ex, "Exception occurred while charging payment gateway for order {OrderId}. Status: FAILED",
                request.OrderId);
        }

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();
        _logger.LogInformation("Payment finalized for order {OrderId}. Final Status: {Status}",
            request.OrderId, payment.Status);

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> GetByOrderAsync(Guid orderId)
    {
        var payment = await _paymentRepository.FindByOrderIdAsync(orderId);
        if (payment is null)
        {
            _logger.LogWarning("Payment not found for order {OrderId}", orderId);
            return null;
        }

        _logger.LogInformation("Retrieved payment for order {OrderId}. PaymentId: {PaymentId}, Status: {Status}",
            orderId, payment.Id, payment.Status);
        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaymentResponseDto>> GetByCustomerAsync(Guid customerId)
    {
        var payments = await _paymentRepository.FindByCustomerIdAsync(customerId);
        _logger.LogInformation("Retrieved {Count} payments for customer {CustomerId}", payments.Count, customerId);
        return payments.Select(MapPaymentToResponse).ToList();
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> RefundPaymentAsync(Guid paymentId, RefundRequestDto request)
    {
        var payment = await _paymentRepository.FindByIdAsync(paymentId);
        if (payment is null)
        {
            _logger.LogWarning("Payment not found for refund. PaymentId: {PaymentId}", paymentId);
            return null;
        }

        _logger.LogInformation("Processing refund for payment {PaymentId} (order {OrderId}). Current status: {Status}, Amount: {Amount}, Mode: {Mode}",
            paymentId, payment.OrderId, payment.Status, payment.Amount, payment.Mode);

        if (payment.Status == PaymentStatus.REFUNDED)
        {
            _logger.LogWarning("Payment already refunded. PaymentId: {PaymentId}, OrderId: {OrderId}",
                paymentId, payment.OrderId);
            throw new InvalidOperationException("Payment is already refunded.");
        }

        if (payment.Status != PaymentStatus.PAID)
        {
            _logger.LogWarning("Cannot refund non-PAID payment. PaymentId: {PaymentId}, OrderId: {OrderId}, Status: {Status}",
                paymentId, payment.OrderId, payment.Status);
            throw new InvalidOperationException("Only PAID payments can be refunded.");
        }

        try
        {
            // Wallet refunds: credit the wallet back. Gateway refunds: hit the SDK.
            if (payment.Mode == PaymentMode.WALLET)
            {
                _logger.LogInformation("Processing wallet refund. PaymentId: {PaymentId}, OrderId: {OrderId}, Amount: {Amount}",
                    paymentId, payment.OrderId, payment.Amount);

                var wallet = await EnsureWalletAsync(payment.CustomerId);
                CreditWallet(wallet, payment.Amount,
                    reference: payment.OrderId.ToString(),
                    description: $"Refund for order {payment.OrderId}: {request.Reason ?? "cancellation"}");

                _walletRepository.UpdateWallet(wallet);
                await _walletRepository.SaveChangesAsync();

                _logger.LogInformation("Wallet refund completed. PaymentId: {PaymentId}, CustomerId: {CustomerId}, NewBalance: {Balance}",
                    paymentId, payment.CustomerId, wallet.Balance);
            }
            else if (payment.Mode != PaymentMode.CASH_ON_DELIVERY)
            {
                // Razorpay / Stripe refund. COD has no upstream charge to refund.
                if (!string.IsNullOrWhiteSpace(payment.TransactionId))
                {
                    _logger.LogInformation("Processing gateway refund. PaymentId: {PaymentId}, OrderId: {OrderId}, TransactionId: {TransactionId}, Amount: {Amount}",
                        paymentId, payment.OrderId, payment.TransactionId, payment.Amount);

                    await _gateway.RefundAsync(payment.TransactionId!, payment.Amount, payment.Currency);

                    _logger.LogInformation("Gateway refund completed. PaymentId: {PaymentId}, OrderId: {OrderId}",
                        paymentId, payment.OrderId);
                }
                else
                {
                    _logger.LogWarning("No transaction ID for gateway refund. PaymentId: {PaymentId}, OrderId: {OrderId}",
                        paymentId, payment.OrderId);
                }
            }
            else
            {
                _logger.LogInformation("Skipping gateway refund. PaymentMode: CASH_ON_DELIVERY. PaymentId: {PaymentId}",
                    paymentId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while processing refund. PaymentId: {PaymentId}, OrderId: {OrderId}",
                paymentId, payment.OrderId);
            throw;
        }

        payment.Status = PaymentStatus.REFUNDED;
        payment.RefundedAt = DateTime.UtcNow;

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();

        _logger.LogInformation("Payment refund finalized. PaymentId: {PaymentId}, OrderId: {OrderId}, Status: REFUNDED",
            paymentId, payment.OrderId);

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid paymentId, UpdatePaymentStatusRequestDto request)
    {
        var payment = await _paymentRepository.FindByIdAsync(paymentId);
        if (payment is null)
        {
            _logger.LogWarning("Payment not found for status update. PaymentId: {PaymentId}", paymentId);
            return null;
        }

        var oldStatus = payment.Status;
        // Use the domain method declared on the spec class diagram.
        payment.UpdatePaymentStatus(request.NewStatus.ToString());

        _logger.LogInformation("Payment status updated. PaymentId: {PaymentId}, OrderId: {OrderId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
            paymentId, payment.OrderId, oldStatus, payment.Status);

        _paymentRepository.UpdatePayment(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapPaymentToResponse(payment);
    }

    // ----- Wallet -----

    /// <inheritdoc />
    public async Task<WalletBalanceResponseDto> GetWalletBalanceAsync(Guid customerId)
    {
        var wallet = await EnsureWalletAsync(customerId);
        _logger.LogInformation("Retrieved wallet balance for customer {CustomerId}. WalletId: {WalletId}, Balance: {Balance}",
            customerId, wallet.Id, wallet.Balance);

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
        {
            _logger.LogWarning("Invalid wallet top-up amount: {Amount} for customer {CustomerId}", request.Amount, request.CustomerId);
            throw new InvalidOperationException("Top-up amount must be greater than zero.");
        }

        if (request.CustomerId == Guid.Empty)
        {
            _logger.LogWarning("CustomerId is empty for wallet top-up");
            throw new InvalidOperationException("CustomerId is required.");
        }

        _logger.LogInformation("Processing wallet top-up for customer {CustomerId}. Amount: {Amount}, Mode: {Mode}",
            request.CustomerId, request.Amount, request.FundingMode);

        var wallet = await EnsureWalletAsync(request.CustomerId);

        // In production the funding charge would also go through the gateway
        // before crediting the wallet. The stub gateway always succeeds.
        if (request.FundingMode != PaymentMode.CASH_ON_DELIVERY)
        {
            try
            {
                _logger.LogInformation("Charging payment gateway for wallet top-up. CustomerId: {CustomerId}, Amount: {Amount}, Mode: {Mode}",
                    request.CustomerId, request.Amount, request.FundingMode);

                var (success, _) = await _gateway.ChargeAsync(
                    orderId: Guid.Empty, // top-ups aren't tied to an order
                    amount: request.Amount,
                    currency: "INR",
                    mode: request.FundingMode);

                if (!success)
                {
                    _logger.LogError("Gateway declined wallet top-up. CustomerId: {CustomerId}, Amount: {Amount}",
                        request.CustomerId, request.Amount);
                    throw new InvalidOperationException("Wallet top-up failed: gateway charge declined.");
                }

                _logger.LogInformation("Gateway charge successful for wallet top-up. CustomerId: {CustomerId}",
                    request.CustomerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while charging for wallet top-up. CustomerId: {CustomerId}",
                    request.CustomerId);
                throw;
            }
        }

        CreditWallet(wallet, request.Amount,
            reference: request.Reference,
            description: $"Top-up via {request.FundingMode}");

        _walletRepository.UpdateWallet(wallet);
        await _walletRepository.SaveChangesAsync();

        _logger.LogInformation("Wallet top-up completed. CustomerId: {CustomerId}, NewBalance: {Balance}",
            request.CustomerId, wallet.Balance);

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
        {
            _logger.LogWarning("Invalid wallet payment amount: {Amount} for order {OrderId}", request.Amount, request.OrderId);
            throw new InvalidOperationException("Amount must be greater than zero.");
        }

        if (request.OrderId == Guid.Empty)
        {
            _logger.LogWarning("OrderId is empty for wallet payment");
            throw new InvalidOperationException("OrderId is required.");
        }

        if (request.CustomerId == Guid.Empty)
        {
            _logger.LogWarning("CustomerId is empty for wallet payment. OrderId: {OrderId}", request.OrderId);
            throw new InvalidOperationException("CustomerId is required.");
        }

        _logger.LogInformation("Processing wallet payment for order {OrderId}. Customer: {CustomerId}, Amount: {Amount}",
            request.OrderId, request.CustomerId, request.Amount);

        // Check for duplicate payment
        var existing = await _paymentRepository.FindByOrderIdAsync(request.OrderId);
        if (existing is not null)
        {
            _logger.LogWarning("Payment already exists for order {OrderId}. Cannot create duplicate payment.",
                request.OrderId);
            throw new InvalidOperationException($"A payment already exists for order {request.OrderId}.");
        }

        var wallet = await EnsureWalletAsync(request.CustomerId);

        // Validate sufficient balance before debiting.
        if (wallet.Balance < request.Amount)
        {
            _logger.LogWarning("Insufficient wallet balance. Customer: {CustomerId}, Required: {Required}, Available: {Available}, OrderId: {OrderId}",
                request.CustomerId, request.Amount, wallet.Balance, request.OrderId);
            throw new InvalidOperationException("Insufficient wallet balance.");
        }

        _logger.LogInformation("Debiting wallet for order {OrderId}. Amount: {Amount}, CurrentBalance: {CurrentBalance}",
            request.OrderId, request.Amount, wallet.Balance);

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

        _logger.LogInformation("Wallet payment completed for order {OrderId}. PaymentId: {PaymentId}, NewBalance: {Balance}",
            request.OrderId, payment.Id, wallet.Balance);

        return MapPaymentToResponse(payment);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatementResponseDto>> GetWalletStatementsAsync(Guid customerId)
    {
        var statements = await _walletRepository.GetStatementsByCustomerIdAsync(customerId);
        _logger.LogInformation("Retrieved {Count} wallet statements for customer {CustomerId}",
            statements.Count, customerId);

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

        _logger.LogInformation("Creating new wallet for customer {CustomerId}", customerId);

        wallet = new Wallet
        {
            CustomerId = customerId,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _walletRepository.AddWalletAsync(wallet);
        await _walletRepository.SaveChangesAsync();

        _logger.LogInformation("Wallet created for customer {CustomerId}. WalletId: {WalletId}",
            customerId, wallet.Id);

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

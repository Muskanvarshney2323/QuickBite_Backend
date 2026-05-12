using QuickBite.Payment.Domain.Entities;
using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.Interfaces;

/// <summary>
/// Repository contract for Payment persistence.
/// Method names mirror the Payment-Service class diagram spec:
/// FindByOrderId, FindByCustomerId, FindByStatus, FindByTransactionId,
/// FindByPaidAtBetween, SumAmountByCustomerId.
/// </summary>
public interface IPaymentRepository
{
    Task<Domain.Entities.Payment?> FindByOrderIdAsync(Guid orderId);

    Task<IReadOnlyList<Domain.Entities.Payment>> FindByCustomerIdAsync(Guid customerId);

    Task<IReadOnlyList<Domain.Entities.Payment>> FindByStatusAsync(PaymentStatus status);

    Task<Domain.Entities.Payment?> FindByTransactionIdAsync(string transactionId);

    Task<IReadOnlyList<Domain.Entities.Payment>> FindByPaidAtBetweenAsync(DateTime fromUtc, DateTime toUtc);

    Task<decimal> SumAmountByCustomerIdAsync(Guid customerId);

    /// <summary>Find a payment by its own id.</summary>
    Task<Domain.Entities.Payment?> FindByIdAsync(Guid paymentId);

    /// <summary>Persist a new payment.</summary>
    Task AddPaymentAsync(Domain.Entities.Payment payment);

    /// <summary>Mark a payment as updated.</summary>
    void UpdatePayment(Domain.Entities.Payment payment);

    /// <summary>Commit pending changes to the database.</summary>
    Task SaveChangesAsync();
}

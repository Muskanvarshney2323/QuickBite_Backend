using Microsoft.EntityFrameworkCore;
using QuickBite.Payment.Application.Interfaces;
using QuickBite.Payment.Domain.Enums;
using QuickBite.Payment.Infrastructure.Data;

namespace QuickBite.Payment.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IPaymentRepository.
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Payment?> FindByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Payment>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _context.Payments
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.PaidAt ?? DateTime.MinValue)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Payment>> FindByStatusAsync(PaymentStatus status)
    {
        return await _context.Payments
            .Where(x => x.Status == status)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Payment?> FindByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Payment>> FindByPaidAtBetweenAsync(DateTime fromUtc, DateTime toUtc)
    {
        return await _context.Payments
            .Where(x => x.PaidAt != null && x.PaidAt >= fromUtc && x.PaidAt <= toUtc)
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<decimal> SumAmountByCustomerIdAsync(Guid customerId)
    {
        return await _context.Payments
            .Where(x => x.CustomerId == customerId && x.Status == PaymentStatus.PAID)
            .SumAsync(x => (decimal?)x.Amount) ?? 0m;
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Payment?> FindByIdAsync(Guid paymentId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.Id == paymentId);
    }

    /// <inheritdoc />
    public async Task AddPaymentAsync(Domain.Entities.Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    /// <inheritdoc />
    public void UpdatePayment(Domain.Entities.Payment payment) => _context.Payments.Update(payment);

    /// <inheritdoc />
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

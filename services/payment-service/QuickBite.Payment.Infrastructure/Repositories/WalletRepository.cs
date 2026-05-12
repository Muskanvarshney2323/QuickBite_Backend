using Microsoft.EntityFrameworkCore;
using QuickBite.Payment.Application.Interfaces;
using QuickBite.Payment.Domain.Entities;
using QuickBite.Payment.Infrastructure.Data;

namespace QuickBite.Payment.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IWalletRepository.
/// </summary>
public class WalletRepository : IWalletRepository
{
    private readonly PaymentDbContext _context;

    public WalletRepository(PaymentDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Wallet?> FindByCustomerIdAsync(Guid customerId)
    {
        return await _context.Wallets
            .Include(x => x.WalletStatements)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatement>> GetStatementsByCustomerIdAsync(Guid customerId)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (wallet is null) return Array.Empty<WalletStatement>();

        return await _context.WalletStatements
            .Where(s => s.WalletId == wallet.Id)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddWalletAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }

    /// <inheritdoc />
    public async Task AddStatementAsync(WalletStatement statement)
    {
        await _context.WalletStatements.AddAsync(statement);
    }

    /// <inheritdoc />
    public void UpdateWallet(Wallet wallet) => _context.Wallets.Update(wallet);

    /// <inheritdoc />
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

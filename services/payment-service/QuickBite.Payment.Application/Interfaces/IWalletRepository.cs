using QuickBite.Payment.Domain.Entities;

namespace QuickBite.Payment.Application.Interfaces;

/// <summary>
/// Repository contract for Wallet and WalletStatement persistence.
/// </summary>
public interface IWalletRepository
{
    /// <summary>Find a customer's wallet (or null if they don't have one yet).</summary>
    Task<Wallet?> FindByCustomerIdAsync(Guid customerId);

    /// <summary>All ledger entries for a customer's wallet, newest first.</summary>
    Task<IReadOnlyList<WalletStatement>> GetStatementsByCustomerIdAsync(Guid customerId);

    /// <summary>Persist a new wallet.</summary>
    Task AddWalletAsync(Wallet wallet);

    /// <summary>Persist a new wallet statement entry.</summary>
    Task AddStatementAsync(WalletStatement statement);

    /// <summary>Mark a wallet as updated.</summary>
    void UpdateWallet(Wallet wallet);

    /// <summary>Commit pending changes to the database.</summary>
    Task SaveChangesAsync();
}

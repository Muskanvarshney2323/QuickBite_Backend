using QuickBite.Payment.Domain.Common;

namespace QuickBite.Payment.Domain.Entities;

/// <summary>
/// Tracks a customer's running e-wallet balance, with a 1..* relationship
/// to WalletStatement entries that record every deposit and debit.
/// </summary>
public class Wallet : BaseEntity
{
    /// <summary>Owner of the wallet. One wallet per customer.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Current spendable balance.</summary>
    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Ledger of every credit and debit against this wallet.</summary>
    public ICollection<WalletStatement> WalletStatements { get; set; } = new List<WalletStatement>();
}

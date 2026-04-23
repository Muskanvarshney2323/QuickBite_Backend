using QuickBite.Payment.Domain.Common;
using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Domain.Entities;

/// <summary>
/// A single ledger entry against a Wallet — either a deposit (top-up) or a debit (spend).
/// </summary>
public class WalletStatement : BaseEntity
{
    /// <summary>Foreign key to the parent wallet.</summary>
    public Guid WalletId { get; set; }

    /// <summary>Direction of the entry: DEPOSIT or DEBIT.</summary>
    public WalletStatementType Type { get; set; }

    /// <summary>Amount of the deposit / debit (always positive; direction is carried by Type).</summary>
    public decimal Amount { get; set; }

    /// <summary>Wallet balance immediately after this entry was applied.</summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>Optional reference to an upstream transaction (gateway txn id, order id, etc.).</summary>
    public string? Reference { get; set; }

    /// <summary>Optional human-readable note ("Top-up via UPI", "Order #123 payment", "Refund").</summary>
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation property to the parent wallet.</summary>
    public Wallet? Wallet { get; set; }
}

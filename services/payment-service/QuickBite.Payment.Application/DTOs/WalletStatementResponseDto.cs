using QuickBite.Payment.Domain.Enums;

namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Response DTO for a single wallet ledger entry.
/// </summary>
public class WalletStatementResponseDto
{
    public Guid StatementId { get; set; }

    public WalletStatementType Type { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; set; }

    public string? Reference { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}

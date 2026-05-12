namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Response DTO for a customer's wallet balance.
/// </summary>
public class WalletBalanceResponseDto
{
    public Guid WalletId { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Balance { get; set; }
}

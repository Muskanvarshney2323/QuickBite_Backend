namespace QuickBite.Payment.Application.DTOs;

/// <summary>
/// Request DTO used to pay for an order using the customer's wallet balance.
/// The service validates that the wallet has sufficient balance before debiting.
/// </summary>
public class PayFromWalletRequestDto
{
    public Guid CustomerId { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }
}

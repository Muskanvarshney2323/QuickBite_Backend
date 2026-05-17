// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Payment.Application.DTOs;

// ========================= WALLET BALANCE RESPONSE DTO =========================
/// <summary>
/// WalletBalanceResponseDto: Response DTO for customer's e-wallet balance
/// Used in GET /api/v1/wallet/balance endpoint
/// Returns current wallet balance and wallet metadata
/// </summary>
public class WalletBalanceResponseDto
{
    // ========================= WALLET ID =========================
    // WalletId: Unique identifier for this customer's wallet
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // One-to-one relationship with customer account
    public Guid WalletId { get; set; }

    // ========================= CUSTOMER ID =========================
    // CustomerId: Customer who owns this wallet
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links wallet to customer account
    public Guid CustomerId { get; set; }

    // ========================= BALANCE =========================
    // Balance: Current available balance in customer's e-wallet
    // Type: Decimal (currency amount)
    // Example: 1500.50 (for ₹1500.50)
    // Business Logic: Updated when customer:
    //   - Adds funds (top-up)
    //   - Makes payment from wallet
    //   - Receives refund
    //   - Receives promotional credits
    // Used for: Wallet payment validation
    public decimal Balance { get; set; }
}

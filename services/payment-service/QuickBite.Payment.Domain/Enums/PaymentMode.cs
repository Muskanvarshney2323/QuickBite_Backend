namespace QuickBite.Payment.Domain.Enums;

/// <summary>
/// Supported payment modes for orders and wallet transactions.
/// Values are aligned with Order-Service so cross-service enum serialization never maps to the wrong mode.
/// CASH_ON_DELIVERY = 1: Pay at delivery
/// CARD = 2: Credit/Debit card payment
/// UPI = 3: Unified Payments Interface
/// WALLET = 4: Customer e-wallet
/// </summary>
public enum PaymentMode
{
    CASH_ON_DELIVERY = 1,
    CARD = 2,
    UPI = 3,
    WALLET = 4
}

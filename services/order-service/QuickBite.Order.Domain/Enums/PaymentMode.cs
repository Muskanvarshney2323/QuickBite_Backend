namespace QuickBite.Order.Domain.Enums;

/// <summary>
/// Payment modes accepted at order placement.
/// Values must match exactly with Payment-Service to prevent enum mismatch during serialization.
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

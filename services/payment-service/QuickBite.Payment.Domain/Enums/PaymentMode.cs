namespace QuickBite.Payment.Domain.Enums;

/// <summary>
/// Supported payment modes per the Payment-Service spec.
/// </summary>
public enum PaymentMode
{
    CARD = 0,
    UPI = 1,
    WALLET = 2,
    COD = 3
}

namespace QuickBite.Payment.Domain.Enums;

/// <summary>
/// Lifecycle status of a payment, exactly as listed in the Payment-Service spec.
/// </summary>
public enum PaymentStatus
{
    PENDING = 0,
    PAID = 1,
    REFUNDED = 2,
    FAILED = 3
}

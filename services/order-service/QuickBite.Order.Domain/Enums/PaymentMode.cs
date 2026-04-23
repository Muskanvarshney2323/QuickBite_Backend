namespace QuickBite.Order.Domain.Enums;

/// <summary>
/// Payment modes accepted at order placement.
/// Values mirror the Payment-Service options.
/// </summary>
public enum PaymentMode
{
    CASH_ON_DELIVERY = 0,
    UPI = 1,
    CARD = 2,
    WALLET = 3,
    NET_BANKING = 4
}

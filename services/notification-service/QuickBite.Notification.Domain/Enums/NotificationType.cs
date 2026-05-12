namespace QuickBite.Notification.Domain.Enums;

/// <summary>
/// What the notification is about.
/// Stored as an int in the database.
/// </summary>
public enum NotificationType
{
    ORDER = 0,    // order lifecycle updates (placed, confirmed, picked up, delivered)
    PAYMENT = 1,  // payment succeeded, failed, refunded
    PROMO = 2,    // promotional / marketing messages
    DELIVERY = 3  // delivery-specific updates (agent assigned, nearby, etc.)
}

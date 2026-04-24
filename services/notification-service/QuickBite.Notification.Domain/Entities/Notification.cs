using QuickBite.Notification.Domain.Common;
using QuickBite.Notification.Domain.Enums;

namespace QuickBite.Notification.Domain.Entities;

/// <summary>
/// A single notification sent to a recipient.
/// Covers in-app, email, and SMS channels for all order lifecycle events,
/// payment updates, promotional messages, and delivery updates.
/// </summary>
public class Notification : BaseEntity
{
    // Id (from BaseEntity) is the notification id.

    /// <summary>Who receives this notification (user / agent / restaurant id).</summary>
    public Guid RecipientId { get; set; }

    /// <summary>Kind of notification (ORDER, PAYMENT, PROMO, DELIVERY).</summary>
    public NotificationType Type { get; set; } = NotificationType.ORDER;

    /// <summary>Short title shown to the user.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Full body text of the notification.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Which channel this was sent through (APP / EMAIL / SMS).</summary>
    public NotificationChannel Channel { get; set; } = NotificationChannel.APP;

    /// <summary>Optional id of a related entity (e.g. the order this is about).</summary>
    public Guid? RelatedId { get; set; }

    /// <summary>Optional type/label of the related entity (e.g. "Order", "Payment").</summary>
    public string RelatedType { get; set; } = string.Empty;

    /// <summary>Whether the recipient has seen / read this notification.</summary>
    public bool IsRead { get; set; }

    /// <summary>When the notification was sent.</summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

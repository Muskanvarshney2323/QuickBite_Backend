using QuickBite.Notification.Domain.Enums;

// Alias so "NotificationEntity" always means the class,
// not the "QuickBite.Notification" namespace.
using NotificationEntity = QuickBite.Notification.Domain.Entities.Notification;

namespace QuickBite.Notification.Application.Interfaces;

/// <summary>
/// Repository contract for saving and loading notifications from the database.
/// </summary>
public interface INotificationRepository
{
    // Find a notification by its id.
    Task<NotificationEntity?> FindByIdAsync(Guid notificationId);

    // All notifications for a recipient (newest first).
    Task<IReadOnlyList<NotificationEntity>> FindByRecipientIdAsync(Guid recipientId);

    // Notifications for a recipient, filtered by read state.
    Task<IReadOnlyList<NotificationEntity>> FindByRecipientIdAndIsReadAsync(Guid recipientId, bool isRead);

    // Count of unread notifications for a recipient.
    Task<int> CountByRecipientIdAndIsReadAsync(Guid recipientId, bool isRead);

    // All notifications of a specific type (e.g. all PROMO notifications).
    Task<IReadOnlyList<NotificationEntity>> FindByTypeAsync(NotificationType type);

    // All notifications linked to a specific related entity (e.g. all notifications for an order).
    Task<IReadOnlyList<NotificationEntity>> FindByRelatedIdAsync(Guid relatedId);

    // All notifications in the system.
    Task<IReadOnlyList<NotificationEntity>> FindAllAsync();

    // Save a new notification.
    Task AddNotificationAsync(NotificationEntity notification);

    // Save many new notifications at once (used for bulk sends).
    Task AddRangeAsync(IEnumerable<NotificationEntity> notifications);

    // Mark an existing notification as updated.
    void UpdateNotification(NotificationEntity notification);

    // Delete a single notification.
    Task<bool> DeleteByNotificationIdAsync(Guid notificationId);

    // Commit all pending changes to the database.
    Task SaveChangesAsync();
}

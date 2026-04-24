using Microsoft.EntityFrameworkCore;
using QuickBite.Notification.Application.Interfaces;
using QuickBite.Notification.Domain.Enums;
using QuickBite.Notification.Infrastructure.Data;

// Alias so "NotificationEntity" always means the class,
// not the "QuickBite.Notification" namespace.
using NotificationEntity = QuickBite.Notification.Domain.Entities.Notification;

namespace QuickBite.Notification.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the notification repository.
/// All database queries live here.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    // Find a notification by id.
    public async Task<NotificationEntity?> FindByIdAsync(Guid notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);
    }

    // All notifications for a recipient (newest first).
    public async Task<IReadOnlyList<NotificationEntity>> FindByRecipientIdAsync(Guid recipientId)
    {
        return await _context.Notifications
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
    }

    // Notifications for a recipient filtered by read state.
    public async Task<IReadOnlyList<NotificationEntity>> FindByRecipientIdAndIsReadAsync(Guid recipientId, bool isRead)
    {
        return await _context.Notifications
            .Where(n => n.RecipientId == recipientId && n.IsRead == isRead)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
    }

    // Count of read/unread notifications for a recipient.
    public async Task<int> CountByRecipientIdAndIsReadAsync(Guid recipientId, bool isRead)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientId == recipientId && n.IsRead == isRead);
    }

    // All notifications of a given type.
    public async Task<IReadOnlyList<NotificationEntity>> FindByTypeAsync(NotificationType type)
    {
        return await _context.Notifications
            .Where(n => n.Type == type)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
    }

    // All notifications linked to a related entity id.
    public async Task<IReadOnlyList<NotificationEntity>> FindByRelatedIdAsync(Guid relatedId)
    {
        return await _context.Notifications
            .Where(n => n.RelatedId == relatedId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
    }

    // All notifications.
    public async Task<IReadOnlyList<NotificationEntity>> FindAllAsync()
    {
        return await _context.Notifications
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();
    }

    // Add a new notification to the DbSet (not saved until SaveChanges).
    public async Task AddNotificationAsync(NotificationEntity notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    // Add many notifications at once (used for bulk sends).
    public async Task AddRangeAsync(IEnumerable<NotificationEntity> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
    }

    // Mark a notification as updated so EF tracks the changes.
    public void UpdateNotification(NotificationEntity notification)
    {
        _context.Notifications.Update(notification);
    }

    // Delete a notification (returns false if it didn't exist).
    public async Task<bool> DeleteByNotificationIdAsync(Guid notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification is null) return false;

        _context.Notifications.Remove(notification);
        return true;
    }

    // Commit all pending changes to the database.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

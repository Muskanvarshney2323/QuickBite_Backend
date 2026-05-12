using QuickBite.Notification.Application.DTOs;

namespace QuickBite.Notification.Application.Interfaces;

/// <summary>
/// Service contract for notification business logic.
/// Mirrors the Notification-Service class-diagram methods:
/// Send, SendBulk, MarkAsRead, MarkAllRead, GetByRecipient, GetUnreadCount,
/// DeleteNotification, SendEmail, SendSms, GetAll.
/// (Email and SMS are kept simple — they just save a record with the chosen channel.
/// In a real system they'd call MailKit / Twilio here.)
/// </summary>
public interface INotificationService
{
    // Send one notification to one recipient.
    Task<NotificationResponseDto> SendAsync(SendNotificationRequestDto request);

    // Send the same notification to many recipients at once.
    Task<IReadOnlyList<NotificationResponseDto>> SendBulkAsync(SendBulkNotificationRequestDto request);

    // Mark a single notification as read.
    Task<NotificationResponseDto?> MarkAsReadAsync(Guid notificationId);

    // Mark every notification for a recipient as read.
    Task<int> MarkAllReadAsync(Guid recipientId);

    // All notifications for a recipient (newest first).
    Task<IReadOnlyList<NotificationResponseDto>> GetByRecipientAsync(Guid recipientId);

    // How many unread notifications a recipient has.
    Task<int> GetUnreadCountAsync(Guid recipientId);

    // Delete a single notification.
    Task<bool> DeleteNotificationAsync(Guid notificationId);

    // Send an "email" notification (here we just record it with Channel = EMAIL).
    Task<NotificationResponseDto> SendEmailAsync(SendNotificationRequestDto request);

    // Send an "SMS" notification (here we just record it with Channel = SMS).
    Task<NotificationResponseDto> SendSmsAsync(SendNotificationRequestDto request);

    // Every notification in the system.
    Task<IReadOnlyList<NotificationResponseDto>> GetAllAsync();
}

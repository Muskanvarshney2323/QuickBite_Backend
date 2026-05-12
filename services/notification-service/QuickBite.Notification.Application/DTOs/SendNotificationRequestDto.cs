using QuickBite.Notification.Domain.Enums;

namespace QuickBite.Notification.Application.DTOs;

/// <summary>
/// Request DTO for sending a single notification.
/// </summary>
public class SendNotificationRequestDto
{
    public Guid RecipientId { get; set; }
    public NotificationType Type { get; set; } = NotificationType.ORDER;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; } = NotificationChannel.APP;
    public Guid? RelatedId { get; set; }
    public string RelatedType { get; set; } = string.Empty;
}

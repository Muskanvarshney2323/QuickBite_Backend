using QuickBite.Notification.Domain.Enums;

namespace QuickBite.Notification.Application.DTOs;

/// <summary>
/// Request DTO for sending the same notification to many recipients at once
/// (e.g. a promotional broadcast).
/// </summary>
public class SendBulkNotificationRequestDto
{
    public List<Guid> RecipientIds { get; set; } = new();
    public NotificationType Type { get; set; } = NotificationType.PROMO;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; } = NotificationChannel.APP;
    public Guid? RelatedId { get; set; }
    public string RelatedType { get; set; } = string.Empty;
}

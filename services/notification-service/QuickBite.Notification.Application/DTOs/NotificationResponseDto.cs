using QuickBite.Notification.Domain.Enums;

namespace QuickBite.Notification.Application.DTOs;

/// <summary>
/// Response DTO returned by all notification endpoints.
/// </summary>
public class NotificationResponseDto
{
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public Guid? RelatedId { get; set; }
    public string RelatedType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}

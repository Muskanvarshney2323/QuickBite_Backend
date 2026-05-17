// DTO (Data Transfer Object) classes for request/response
using QuickBite.Notification.Application.DTOs;

// Service interface definitions
using QuickBite.Notification.Application.Interfaces;

// Enum types (notification channels, types)
using QuickBite.Notification.Domain.Enums;

// Alias so "NotificationEntity" always means the class,
// not the "QuickBite.Notification" namespace.
using NotificationEntity = QuickBite.Notification.Domain.Entities.Notification;

// Namespace for service classes
namespace QuickBite.Notification.Application.Services;

// ========================= SUMMARY =========================
/// <summary>
/// NotificationService: Business logic for notification management
/// Features:
/// - Send single notifications to recipients
/// - Send bulk notifications to multiple recipients at once
/// - Support for multiple channels (Push, Email, SMS)
/// - Mark notifications as read (single and batch)
/// - Retrieve notifications by recipient
/// - Track unread notification count
/// - Delete notifications
/// </summary>
public class NotificationService : INotificationService
{
    // Repository for accessing Notification data from database
    private readonly INotificationRepository _repository;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    public NotificationService(INotificationRepository repository)
    {
        // Store repository reference
        _repository = repository;
    }

    // Send one notification.
    public async Task<NotificationResponseDto> SendAsync(SendNotificationRequestDto request)
    {
        var notification = BuildEntity(request, request.Channel);

        await _repository.AddNotificationAsync(notification);
        await _repository.SaveChangesAsync();

        return MapToResponse(notification);
    }

    // Send the same notification to many recipients at once.
    public async Task<IReadOnlyList<NotificationResponseDto>> SendBulkAsync(SendBulkNotificationRequestDto request)
    {
        // Build one entity per recipient.
        var notifications = new List<NotificationEntity>();
        foreach (var recipientId in request.RecipientIds)
        {
            var n = new NotificationEntity
            {
                RecipientId = recipientId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                Channel = request.Channel,
                RelatedId = request.RelatedId,
                RelatedType = request.RelatedType,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };
            notifications.Add(n);
        }

        await _repository.AddRangeAsync(notifications);
        await _repository.SaveChangesAsync();

        return notifications.Select(MapToResponse).ToList();
    }

    // Mark a single notification as read.
    public async Task<NotificationResponseDto?> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _repository.FindByIdAsync(notificationId);
        if (notification is null) return null;

        notification.IsRead = true;

        _repository.UpdateNotification(notification);
        await _repository.SaveChangesAsync();

        return MapToResponse(notification);
    }

    // Mark every notification for a recipient as read.
    // Returns the count of notifications that were just marked.
    public async Task<int> MarkAllReadAsync(Guid recipientId)
    {
        var unread = await _repository.FindByRecipientIdAndIsReadAsync(recipientId, false);

        foreach (var n in unread)
        {
            n.IsRead = true;
            _repository.UpdateNotification(n);
        }

        await _repository.SaveChangesAsync();

        return unread.Count;
    }

    // All notifications for a recipient.
    public async Task<IReadOnlyList<NotificationResponseDto>> GetByRecipientAsync(Guid recipientId)
    {
        var notifications = await _repository.FindByRecipientIdAsync(recipientId);
        return notifications.Select(MapToResponse).ToList();
    }

    // How many unread notifications a recipient has.
    public async Task<int> GetUnreadCountAsync(Guid recipientId)
    {
        return await _repository.CountByRecipientIdAndIsReadAsync(recipientId, false);
    }

    // Delete a single notification.
    public async Task<bool> DeleteNotificationAsync(Guid notificationId)
    {
        var deleted = await _repository.DeleteByNotificationIdAsync(notificationId);
        if (!deleted) return false;

        await _repository.SaveChangesAsync();
        return true;
    }

    // Send an "email" — just forces Channel = EMAIL and stores the record.
    public async Task<NotificationResponseDto> SendEmailAsync(SendNotificationRequestDto request)
    {
        var notification = BuildEntity(request, NotificationChannel.EMAIL);

        await _repository.AddNotificationAsync(notification);
        await _repository.SaveChangesAsync();

        return MapToResponse(notification);
    }

    // Send an "SMS" — just forces Channel = SMS and stores the record.
    public async Task<NotificationResponseDto> SendSmsAsync(SendNotificationRequestDto request)
    {
        var notification = BuildEntity(request, NotificationChannel.SMS);

        await _repository.AddNotificationAsync(notification);
        await _repository.SaveChangesAsync();

        return MapToResponse(notification);
    }

    // Every notification in the system.
    public async Task<IReadOnlyList<NotificationResponseDto>> GetAllAsync()
    {
        var notifications = await _repository.FindAllAsync();
        return notifications.Select(MapToResponse).ToList();
    }

    // Helper: build a Notification entity from a request and a chosen channel.
    private static NotificationEntity BuildEntity(SendNotificationRequestDto request, NotificationChannel channel)
    {
        return new NotificationEntity
        {
            RecipientId = request.RecipientId,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            Channel = channel,
            RelatedId = request.RelatedId,
            RelatedType = request.RelatedType,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
    }

    // Helper: convert an entity into the response DTO.
    private static NotificationResponseDto MapToResponse(NotificationEntity n)
    {
        return new NotificationResponseDto
        {
            NotificationId = n.Id,
            RecipientId = n.RecipientId,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            Channel = n.Channel,
            RelatedId = n.RelatedId,
            RelatedType = n.RelatedType,
            IsRead = n.IsRead,
            SentAt = n.SentAt
        };
    }
}

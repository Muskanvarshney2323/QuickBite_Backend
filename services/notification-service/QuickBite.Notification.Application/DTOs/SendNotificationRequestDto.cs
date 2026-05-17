// Import for notification type and channel enums
using QuickBite.Notification.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Notification.Application.DTOs;

// ========================= SEND NOTIFICATION REQUEST DTO =========================
/// <summary>
/// SendNotificationRequestDto: Request DTO to send notification to user
/// Used in POST /api/v1/notifications/send endpoint
/// Triggers single notification to recipient via specified channel
/// </summary>
public class SendNotificationRequestDto
{
    // ========================= RECIPIENT ID =========================
    // RecipientId: User who should receive this notification
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Required: Yes (cannot be empty)
    // Business Logic: Identifies target user for notification
    public Guid RecipientId { get; set; }

    // ========================= TYPE =========================
    /// <summary>
    /// Notification type: ORDER, PAYMENT, DELIVERY, REVIEW, PROMO, SYSTEM
    /// Default is ORDER
    /// </summary>
    // Type: NotificationType enum
    // Example values:
    //   ORDER = Order-related notification (placed, confirmed, ready)
    //   PAYMENT = Payment-related notification (paid, failed, refunded)
    //   DELIVERY = Delivery-related notification (assigned, on the way, delivered)
    //   REVIEW = Review-related notification (request for review)
    //   PROMO = Promotional notification (special offers, discounts)
    //   SYSTEM = System notification (account changes, announcements)
    // Default: ORDER
    // Business Logic: Determines notification categorization and routing
    // Required: No (optional, defaults to ORDER)
    public NotificationType Type { get; set; } = NotificationType.ORDER;

    // ========================= TITLE =========================
    // Title: Short heading for notification
    // Type: String
    // Example: "Order Confirmed!", "Payment Successful"
    // Shown as notification headline
    // Required: Yes (should not be empty)
    public string Title { get; set; } = string.Empty;

    // ========================= MESSAGE =========================
    // Message: Full notification message body
    // Type: String
    // Example: "Your order #12345 has been confirmed by restaurant"
    // Shown as notification content
    // Required: Yes (should not be empty)
    public string Message { get; set; } = string.Empty;

    // ========================= CHANNEL =========================
    /// <summary>
    /// Delivery channel: APP (in-app), EMAIL, SMS, PUSH
    /// Default is APP
    /// </summary>
    // Type: NotificationChannel enum
    // Example values:
    //   APP = In-app notification (browser/app notification center)
    //   EMAIL = Email notification
    //   SMS = SMS/text message
    //   PUSH = Push notification (mobile app push)
    // Default: APP
    // Business Logic: Determines how notification is delivered to user
    // Required: No (optional, defaults to APP)
    public NotificationChannel Channel { get; set; } = NotificationChannel.APP;

    // ========================= RELATED ID =========================
    // RelatedId: ID of entity this notification relates to
    // Type: GUID (nullable)
    // Example: Order ID if this is order notification, Payment ID if payment notification
    // Business Logic: Allows user to jump directly to related resource
    // Required: No (can be null)
    public Guid? RelatedId { get; set; }

    // ========================= RELATED TYPE =========================
    // RelatedType: Type of entity this notification relates to
    // Type: String
    // Example: "ORDER", "PAYMENT", "DELIVERY", "REVIEW"
    // Business Logic: Determines which resource to open when user clicks notification
    // Required: No (can be null or empty)
    public string RelatedType { get; set; } = string.Empty;
}

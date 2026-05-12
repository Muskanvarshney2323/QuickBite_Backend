namespace QuickBite.Notification.Domain.Enums;

/// <summary>
/// Which channel the notification is delivered over.
/// Stored as an int in the database.
/// </summary>
public enum NotificationChannel
{
    APP = 0,    // in-app notification
    EMAIL = 1,  // email
    SMS = 2     // text message
}

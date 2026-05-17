// Used for authorization features like [Authorize] attribute
using Microsoft.AspNetCore.Authorization;

// Used for Web API controller features, routing, IActionResult, etc.
using Microsoft.AspNetCore.Mvc;

// DTO (Data Transfer Object) classes for request/response
using QuickBite.Notification.Application.DTOs;

// Service interface for business logic
using QuickBite.Notification.Application.Interfaces;

// Used for Swagger/OpenAPI documentation
using Swashbuckle.AspNetCore.Annotations;

// Namespace for this controller
namespace QuickBite.Notification.API.Controllers;

// ========================= NOTIFICATION CONTROLLER SUMMARY =========================
/// <summary>
/// NotificationController: Exposes HTTP endpoints for notification operations
/// Supports multiple channels: Push, Email, SMS
/// Endpoints: send single/bulk, email, SMS, get by recipient, mark as read, get unread count, delete, get all
/// </summary>

// ========================= ATTRIBUTES =========================

// Mark this class as an API Controller (enables automatic model validation)
[ApiController]

// Base route for all endpoints in this controller
// Example: POST /api/v1/notifications
[Route("api/v1/notifications")]

// Requires JWT authentication token for all endpoints
[Authorize]

// Swagger documentation tag
[SwaggerTag("Notification operations: send (single/bulk/email/sms), get by recipient, mark as read, unread count, delete, get all")]
public class NotificationController : ControllerBase
{
    // Service object that contains all notification business logic
    private readonly INotificationService _notificationService;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    // ASP.NET automatically injects INotificationService from DI container
    public NotificationController(INotificationService notificationService)
    {
        // Store service reference for use in endpoint methods
        _notificationService = notificationService;
    }

    // Send one notification.
    [HttpPost]
    [SwaggerOperation(Summary = "Send notification")]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequestDto request)
    {
        var result = await _notificationService.SendAsync(request);
        return CreatedAtAction(nameof(GetByRecipient), new { recipientId = result.RecipientId }, result);
    }

    // Send the same notification to many recipients at once.
    [HttpPost("bulk")]
    [SwaggerOperation(Summary = "Send bulk notification")]
    public async Task<IActionResult> SendBulk([FromBody] SendBulkNotificationRequestDto request)
    {
        var result = await _notificationService.SendBulkAsync(request);
        return Ok(result);
    }

    // Send an email notification.
    [HttpPost("email")]
    [SwaggerOperation(Summary = "Send email notification")]
    public async Task<IActionResult> SendEmail([FromBody] SendNotificationRequestDto request)
    {
        var result = await _notificationService.SendEmailAsync(request);
        return Ok(result);
    }

    // Send an SMS notification.
    [HttpPost("sms")]
    [SwaggerOperation(Summary = "Send SMS notification")]
    public async Task<IActionResult> SendSms([FromBody] SendNotificationRequestDto request)
    {
        var result = await _notificationService.SendSmsAsync(request);
        return Ok(result);
    }

    // Get all notifications for a recipient.
    [HttpGet("recipient/{recipientId:guid}")]
    [SwaggerOperation(Summary = "Get notifications by recipient")]
    public async Task<IActionResult> GetByRecipient(Guid recipientId)
    {
        var result = await _notificationService.GetByRecipientAsync(recipientId);
        return Ok(result);
    }

    // Get the count of unread notifications for a recipient.
    [HttpGet("recipient/{recipientId:guid}/unreadCount")]
    [SwaggerOperation(Summary = "Get unread count")]
    public async Task<IActionResult> GetUnreadCount(Guid recipientId)
    {
        var count = await _notificationService.GetUnreadCountAsync(recipientId);
        return Ok(new { recipientId, unreadCount = count });
    }

    // Mark a single notification as read.
    [HttpPut("{notificationId:guid}/read")]
    [SwaggerOperation(Summary = "Mark as read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        var result = await _notificationService.MarkAsReadAsync(notificationId);
        if (result is null) return NotFound("Notification not found.");
        return Ok(result);
    }

    // Mark every notification for a recipient as read.
    [HttpPut("recipient/{recipientId:guid}/readAll")]
    [SwaggerOperation(Summary = "Mark all read for a recipient")]
    public async Task<IActionResult> MarkAllRead(Guid recipientId)
    {
        var count = await _notificationService.MarkAllReadAsync(recipientId);
        return Ok(new { recipientId, markedCount = count });
    }

    // Delete a notification.
    [HttpDelete("{notificationId:guid}")]
    [SwaggerOperation(Summary = "Delete notification")]
    public async Task<IActionResult> Delete(Guid notificationId)
    {
        var deleted = await _notificationService.DeleteNotificationAsync(notificationId);
        if (!deleted) return NotFound("Notification not found.");
        return NoContent();
    }

    // Get every notification in the system.
    [HttpGet("all")]
    [SwaggerOperation(Summary = "Get all notifications")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _notificationService.GetAllAsync();
        return Ok(result);
    }
}

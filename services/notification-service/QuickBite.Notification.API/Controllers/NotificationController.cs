using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Notification.Application.DTOs;
using QuickBite.Notification.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Notification.API.Controllers;

/// <summary>
/// API endpoints for notifications.
/// Route: /api/v1/notifications
/// </summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
[SwaggerTag("Notification operations: send (single/bulk/email/sms), get by recipient, mark as read, unread count, delete, get all")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
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

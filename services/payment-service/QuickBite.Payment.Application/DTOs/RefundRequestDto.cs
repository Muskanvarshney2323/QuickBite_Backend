// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Payment.Application.DTOs;

// ========================= REFUND REQUEST DTO =========================
/// <summary>
/// RefundRequestDto: Request DTO to refund a previously captured payment
/// Used in POST /api/v1/payments/{paymentId}/refund endpoint
/// Initiates refund process for payment that was already paid
/// </summary>
public class RefundRequestDto
{
    // ========================= REASON =========================
    // Reason: Optional reason for refunding this payment
    // Type: String (nullable)
    // Example: "Customer requested cancellation", "Order was cancelled by restaurant"
    // Business Logic: Recorded in payment history for audit trail
    // Used for: Support communication and refund tracking
    // Required: No (can be null or empty)
    public string? Reason { get; set; }
}

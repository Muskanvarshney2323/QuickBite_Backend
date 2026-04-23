using QuickBite.Order.Domain.Common;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Domain.Entities;

/// <summary>
/// Represents a confirmed order placed by a customer.
/// An Order is created from a confirmed cart and snapshots all line items,
/// pricing, payment mode, and delivery details at the moment of placement.
/// The order is then walked through its lifecycle via UpdateStatus.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Customer who placed the order.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Restaurant fulfilling the order.</summary>
    public Guid RestaurantId { get; set; }

    /// <summary>Delivery agent assigned by the Delivery-Service (null until assigned).</summary>
    public Guid? DeliveryAgentId { get; set; }

    /// <summary>Sum of (Price * Quantity) across all OrderItems.</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Discount applied (e.g. via promo code).</summary>
    public decimal Discount { get; set; }

    /// <summary>Final amount payable after discount = TotalAmount - Discount.</summary>
    public decimal FinalAmount { get; set; }

    /// <summary>How the customer is paying (UPI, card, COD, etc.).</summary>
    public PaymentMode ModeOfPayment { get; set; }

    /// <summary>Current lifecycle status (PLACED -> CONFIRMED -> PREPARING -> PICKED_UP -> DELIVERED, or CANCELLED).</summary>
    public OrderStatus OrderStatus { get; set; } = OrderStatus.PLACED;

    /// <summary>When the order was placed.</summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>Estimated delivery time computed at placement.</summary>
    public DateTime? EstimatedDelivery { get; set; }

    /// <summary>Address to deliver to.</summary>
    public string DeliveryAddress { get; set; } = string.Empty;

    /// <summary>Optional special instructions from the customer.</summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>Snapshot of items in this order (immutable once order is placed).</summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Update the order's lifecycle status. Domain method declared on the spec class diagram.
    /// </summary>
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.", nameof(newStatus));

        if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Unknown order status '{newStatus}'.", nameof(newStatus));

        OrderStatus = parsed;
    }
}

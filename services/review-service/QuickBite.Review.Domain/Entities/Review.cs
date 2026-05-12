using QuickBite.Review.Domain.Common;

namespace QuickBite.Review.Domain.Entities;

/// <summary>
/// A customer review for one completed order.
/// Has two ratings: one for the restaurant (food) and one for the delivery agent.
/// Only one review per order.
/// </summary>
public class Review : BaseEntity
{
    // Id (from BaseEntity) is the review id.

    /// <summary>The order this review is for. Unique so one order has at most one review.</summary>
    public Guid OrderId { get; set; }

    /// <summary>The customer who wrote the review.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>The restaurant being reviewed.</summary>
    public Guid RestaurantId { get; set; }

    /// <summary>The delivery agent being reviewed.</summary>
    public Guid AgentId { get; set; }

    /// <summary>Food quality rating (1 to 5).</summary>
    public int FoodRating { get; set; }

    /// <summary>Delivery experience rating (1 to 5).</summary>
    public int DeliveryRating { get; set; }

    /// <summary>Optional written comment from the customer.</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>When the review was written.</summary>
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    /// <summary>Whether this review has been moderated and verified.</summary>
    public bool IsVerified { get; set; }
}

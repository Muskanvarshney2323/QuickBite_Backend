namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Request DTO used to switch a customer's cart to a new restaurant.
/// This clears all existing items because single-restaurant ordering
/// is enforced at the cart level.
/// </summary>
public class ChangeRestaurantRequestDto
{
    public Guid CustomerId { get; set; }

    public Guid NewRestaurantId { get; set; }
}

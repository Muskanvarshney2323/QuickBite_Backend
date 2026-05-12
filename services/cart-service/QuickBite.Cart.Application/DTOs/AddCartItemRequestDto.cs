namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Request DTO used to add a new item to a customer's cart.
/// If the customer has no cart, one is created bound to this RestaurantId.
/// If the customer already has a cart for a different restaurant,
/// the service rejects the request (see ChangeRestaurant to switch).
/// </summary>
public class AddCartItemRequestDto
{
    public Guid CustomerId { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price snapshot captured at the moment of add.
    /// </summary>
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Optional customisation notes (e.g. "no onions").
    /// </summary>
    public string? Customization { get; set; }
}

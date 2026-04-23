namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Request DTO for applying a promo code to a customer's cart.
/// </summary>
public class ApplyPromoCodeRequestDto
{
    public Guid CustomerId { get; set; }

    public string PromoCode { get; set; } = string.Empty;
}

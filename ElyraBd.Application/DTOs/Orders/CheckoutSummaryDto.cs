using ElyraBd.Application.DTOs.Carts;

namespace ElyraBd.Application.DTOs.Orders;

public class CheckoutSummaryDto
{
    public CartDto Cart { get; set; } = null!;
    public decimal DeliveryCharge { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? AppliedCouponCode { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsFreeDelivery { get; set; }
}

using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductOffer> Offers { get; set; } = new List<ProductOffer>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<ProductViewHistory> ViewHistories { get; set; } = new List<ProductViewHistory>();

    public decimal SalePrice => DiscountPercent > 0
        ? Math.Round(Price * (1 - DiscountPercent / 100m), 2)
        : Price;

    public bool HasDiscount => DiscountPercent > 0;
}

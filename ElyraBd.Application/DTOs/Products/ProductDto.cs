namespace ElyraBd.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal SalePrice { get; set; }
    public bool HasDiscount { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? PrimaryImageUrl { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

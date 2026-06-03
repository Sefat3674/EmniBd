using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class ProductOffer : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string? Title { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

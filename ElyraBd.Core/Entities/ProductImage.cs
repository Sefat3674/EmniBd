using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class ProductViewHistory : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; } = 1;
}

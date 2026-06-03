using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true;
}

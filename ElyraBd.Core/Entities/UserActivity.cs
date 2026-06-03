using ElyraBd.Core.Common;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Entities;

public class UserActivity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public ActivityType ActivityType { get; set; }
    public string? Description { get; set; }
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public int? OrderId { get; set; }
    public string? IpAddress { get; set; }
}

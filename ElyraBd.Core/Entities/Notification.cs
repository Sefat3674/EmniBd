using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}

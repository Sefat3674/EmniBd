using ElyraBd.Core.Common;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public OrderStatus Status { get; set; }
    public string? Note { get; set; }
    public string? ChangedByUserId { get; set; }
}

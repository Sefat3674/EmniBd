using ElyraBd.Core.Enums;

namespace ElyraBd.Application.DTOs.Orders;

public class OrderStatusTimelineItemDto
{
    public OrderStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Note { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCurrent { get; set; }
}

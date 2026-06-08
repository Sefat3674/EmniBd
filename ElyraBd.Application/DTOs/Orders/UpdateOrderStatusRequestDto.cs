using ElyraBd.Core.Enums;

namespace ElyraBd.Application.DTOs.Orders;

public class UpdateOrderStatusRequestDto
{
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Note { get; set; }
}

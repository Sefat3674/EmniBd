using ElyraBd.Core.Enums;

namespace ElyraBd.Application.DTOs.Orders;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public int ItemCount { get; set; }
}

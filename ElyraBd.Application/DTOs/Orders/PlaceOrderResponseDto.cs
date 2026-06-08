namespace ElyraBd.Application.DTOs.Orders;

public class PlaceOrderResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public OrderDetailDto? Order { get; set; }
}

namespace ElyraBd.Application.DTOs.Carts;

public class CartDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IReadOnlyList<CartItemDto> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public decimal SubTotal { get; set; }
}

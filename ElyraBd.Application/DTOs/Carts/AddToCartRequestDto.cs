namespace ElyraBd.Application.DTOs.Carts;

public class AddToCartRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

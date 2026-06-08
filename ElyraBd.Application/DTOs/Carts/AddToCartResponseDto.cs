namespace ElyraBd.Application.DTOs.Carts;

public class AddToCartResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool QuantityIncreased { get; set; }
    public int ItemQuantity { get; set; }
    public CartDto? Cart { get; set; }
}

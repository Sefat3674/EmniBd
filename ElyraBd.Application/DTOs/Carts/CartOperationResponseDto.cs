namespace ElyraBd.Application.DTOs.Carts;

public class CartOperationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public CartDto? Cart { get; set; }
}

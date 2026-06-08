using ElyraBd.Application.DTOs.Carts;

namespace ElyraBd.Application.Interfaces;

public interface ICartService
{
    Task<AddToCartResponseDto> AddToCartAsync(
        string userId,
        AddToCartRequestDto request,
        CancellationToken cancellationToken = default);

    Task<CartDto?> GetCartAsync(string userId, CancellationToken cancellationToken = default);

    Task<CartOperationResponseDto> UpdateQuantityAsync(
        string userId,
        UpdateCartItemRequestDto request,
        CancellationToken cancellationToken = default);

    Task<CartOperationResponseDto> RemoveItemAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default);
}

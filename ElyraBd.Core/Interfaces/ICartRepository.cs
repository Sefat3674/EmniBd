using ElyraBd.Core.Entities;

namespace ElyraBd.Core.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetByUserIdWithItemsAsync(string userId, CancellationToken cancellationToken = default);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId, CancellationToken cancellationToken = default);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken = default);
    Task<CartItem> AddCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task UpdateCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task RemoveCartItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task ClearCartItemsAsync(int cartId, CancellationToken cancellationToken = default);
}

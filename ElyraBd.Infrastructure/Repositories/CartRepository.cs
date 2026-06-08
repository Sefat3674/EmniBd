using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdWithItemsAsync(string userId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId, CancellationToken cancellationToken = default) =>
        await Context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId, cancellationToken);

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken = default) =>
        await Context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId, cancellationToken);

    public async Task<CartItem> AddCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        await Context.CartItems.AddAsync(item, cancellationToken);
        return item;
    }

    public Task UpdateCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        item.UpdatedAt = DateTime.UtcNow;
        Context.CartItems.Update(item);
        return Task.CompletedTask;
    }

    public Task RemoveCartItemAsync(CartItem item, CancellationToken cancellationToken = default)
    {
        Context.CartItems.Remove(item);
        return Task.CompletedTask;
    }

    public async Task ClearCartItemsAsync(int cartId, CancellationToken cancellationToken = default)
    {
        var items = await Context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync(cancellationToken);
        Context.CartItems.RemoveRange(items);
    }
}

using AutoMapper;
using ElyraBd.Application.DTOs.Carts;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IActivityTrackingService _activityTracking;

    public CartService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IActivityTrackingService activityTracking)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _activityTracking = activityTracking;
    }

    public async Task<AddToCartResponseDto> AddToCartAsync(
        string userId,
        AddToCartRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity < 1)
        {
            return new AddToCartResponseDto
            {
                Success = false,
                Message = "Quantity must be at least 1."
            };
        }

        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null || !product.IsActive)
        {
            return new AddToCartResponseDto
            {
                Success = false,
                Message = "Product not found or unavailable."
            };
        }

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var existingItem = await _unitOfWork.Carts.GetCartItemAsync(cart.Id, request.ProductId, cancellationToken);

        var newTotalQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;
        if (newTotalQuantity > product.Stock)
        {
            return new AddToCartResponseDto
            {
                Success = false,
                Message = $"Only {product.Stock} item(s) available in stock."
            };
        }

        var quantityIncreased = existingItem is not null;

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
            await _unitOfWork.Carts.UpdateCartItemAsync(existingItem, cancellationToken);
        }
        else
        {
            await _unitOfWork.Carts.AddCartItemAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = product.SalePrice,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _activityTracking.TrackAsync(
            userId,
            ActivityType.AddToCart,
            quantityIncreased ? "Increased cart quantity" : "Added product to cart",
            request.ProductId,
            cancellationToken: cancellationToken);

        var updatedCart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        var cartDto = _mapper.Map<CartDto>(updatedCart);
        var itemQuantity = updatedCart?.Items.FirstOrDefault(i => i.ProductId == request.ProductId)?.Quantity ?? request.Quantity;

        return new AddToCartResponseDto
        {
            Success = true,
            Message = quantityIncreased
                ? "Product quantity updated in your cart."
                : "Product added to your cart.",
            QuantityIncreased = quantityIncreased,
            ItemQuantity = itemQuantity,
            Cart = cartDto
        };
    }

    public async Task<CartDto?> GetCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        return cart is null ? null : _mapper.Map<CartDto>(cart);
    }

    public async Task<CartOperationResponseDto> UpdateQuantityAsync(
        string userId,
        UpdateCartItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity < 1)
            return Fail("Quantity must be at least 1.");

        var cartItem = await _unitOfWork.Carts.GetCartItemByIdAsync(request.CartItemId, cancellationToken);
        if (cartItem is null || cartItem.Cart.UserId != userId)
            return Fail("Cart item not found.");

        var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId, cancellationToken);
        if (product is null || !product.IsActive)
            return Fail("Product is no longer available.");

        if (request.Quantity > product.Stock)
            return Fail($"Only {product.Stock} item(s) available in stock.");

        cartItem.Quantity = request.Quantity;
        cartItem.UnitPrice = product.SalePrice;
        await _unitOfWork.Carts.UpdateCartItemAsync(cartItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await SuccessCartAsync(userId, "Cart updated.", cancellationToken);
    }

    public async Task<CartOperationResponseDto> RemoveItemAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        var cartItem = await _unitOfWork.Carts.GetCartItemByIdAsync(cartItemId, cancellationToken);
        if (cartItem is null)
            return Fail("Cart item not found.");

        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (cart is null || cartItem.CartId != cart.Id)
            return Fail("Cart item not found.");

        var productId = cartItem.ProductId;
        await _unitOfWork.Carts.RemoveCartItemAsync(cartItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _activityTracking.TrackAsync(
            userId,
            ActivityType.RemoveFromCart,
            "Removed product from cart",
            productId,
            cancellationToken: cancellationToken);

        return await SuccessCartAsync(userId, "Item removed from cart.", cancellationToken);
    }

    private async Task<CartOperationResponseDto> SuccessCartAsync(
        string userId,
        string message,
        CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        return new CartOperationResponseDto
        {
            Success = true,
            Message = message,
            Cart = cart is null ? null : _mapper.Map<CartDto>(cart)
        };
    }

    private static CartOperationResponseDto Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    private async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (cart is not null)
            return cart;

        cart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return cart;
    }
}

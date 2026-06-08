using ElyraBd.Application.DTOs.Orders;
using ElyraBd.Core.Enums;

namespace ElyraBd.Application.Interfaces;

public interface IOrderService
{
    Task<CheckoutSummaryDto?> GetCheckoutSummaryAsync(string userId, string? couponCode, CancellationToken cancellationToken = default);
    Task<PlaceOrderResponseDto> PlaceOrderAsync(string userId, PlaceOrderRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderSummaryDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetOrderDetailAsync(string userId, int orderId, CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetOrderDetailForAdminAsync(int orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderSummaryDto>> GetAllOrdersAsync(OrderStatus? status, CancellationToken cancellationToken = default);
    Task<PlaceOrderResponseDto> UpdateOrderStatusAsync(string adminUserId, UpdateOrderStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<PlaceOrderResponseDto> CancelOrderAsync(string userId, int orderId, string reason, bool isAdmin, CancellationToken cancellationToken = default);
}

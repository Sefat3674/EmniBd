using AutoMapper;
using ElyraBd.Application.DTOs.Orders;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Constants;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class OrderService : IOrderService
{
    private static readonly OrderStatus[] TimelineStatuses =
    [
        OrderStatus.Pending,
        OrderStatus.Processing,
        OrderStatus.Packed,
        OrderStatus.Shipped,
        OrderStatus.Delivered
    ];

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICouponService _couponService;
    private readonly INotificationService _notificationService;
    private readonly IActivityTrackingService _activityTracking;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICouponService couponService,
        INotificationService notificationService,
        IActivityTrackingService activityTracking)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _couponService = couponService;
        _notificationService = notificationService;
        _activityTracking = activityTracking;
    }

    public async Task<CheckoutSummaryDto?> GetCheckoutSummaryAsync(
        string userId,
        string? couponCode,
        CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            return null;

        var cartDto = _mapper.Map<DTOs.Carts.CartDto>(cart);
        var subTotal = cartDto.SubTotal;
        var deliveryCharge = subTotal >= CommerceConstants.FreeDeliveryThreshold
            ? 0m
            : CommerceConstants.StandardDeliveryCharge;

        decimal discountAmount = 0;
        string? appliedCode = null;

        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var validation = await _couponService.ValidateAsync(couponCode, subTotal, cancellationToken);
            if (validation.IsValid)
            {
                discountAmount = validation.DiscountAmount;
                appliedCode = validation.Code;
            }
        }

        return new CheckoutSummaryDto
        {
            Cart = cartDto,
            DeliveryCharge = deliveryCharge,
            DiscountAmount = discountAmount,
            AppliedCouponCode = appliedCode,
            TotalAmount = subTotal + deliveryCharge - discountAmount,
            IsFreeDelivery = deliveryCharge == 0
        };
    }

    public async Task<PlaceOrderResponseDto> PlaceOrderAsync(
        string userId,
        PlaceOrderRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.AddressLine) ||
            string.IsNullOrWhiteSpace(request.City))
        {
            return Fail("Please fill in all required delivery details.");
        }

        var cart = await _unitOfWork.Carts.GetByUserIdWithItemsAsync(userId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            return Fail("Your cart is empty.");

        foreach (var item in cart.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null || !product.IsActive)
                return Fail($"Product '{item.Product?.Name ?? "Unknown"}' is no longer available.");

            if (item.Quantity > product.Stock)
                return Fail($"Insufficient stock for '{product.Name}'. Only {product.Stock} available.");
        }

        var subTotal = cart.Items.Sum(i => i.Quantity * i.UnitPrice);
        var deliveryCharge = subTotal >= CommerceConstants.FreeDeliveryThreshold
            ? 0m
            : CommerceConstants.StandardDeliveryCharge;

        decimal discountAmount = 0;
        Coupon? appliedCoupon = null;

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var validation = await _couponService.ValidateAsync(request.CouponCode, subTotal, cancellationToken);
            if (!validation.IsValid)
                return Fail(validation.Message);

            discountAmount = validation.DiscountAmount;
            appliedCoupon = await _unitOfWork.Coupons.GetByCodeAsync(request.CouponCode, cancellationToken);
        }

        var totalAmount = subTotal + deliveryCharge - discountAmount;
        var orderNumber = GenerateOrderNumber();

        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            OrderDate = DateTime.UtcNow,
            SubTotal = subTotal,
            DeliveryCharge = deliveryCharge,
            DiscountAmount = discountAmount,
            CouponCode = appliedCoupon?.Code,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var orderItemRepo = _unitOfWork.Repository<OrderItem>();
        foreach (var cartItem in cart.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId, cancellationToken);
            await orderItemRepo.AddAsync(new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                ProductName = product!.Name,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            product.Stock -= cartItem.Quantity;
            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        }

        var addressRepo = _unitOfWork.Repository<ShippingAddress>();
        await addressRepo.AddAsync(new ShippingAddress
        {
            UserId = userId,
            OrderId = order.Id,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            AddressLine = request.AddressLine.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = string.IsNullOrWhiteSpace(request.Country) ? "Bangladesh" : request.Country.Trim(),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        var paymentRepo = _unitOfWork.Repository<Payment>();
        await paymentRepo.AddAsync(new Payment
        {
            OrderId = order.Id,
            PaymentMethod = request.PaymentMethod,
            Amount = totalAmount,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        var historyRepo = _unitOfWork.Repository<OrderStatusHistory>();
        await historyRepo.AddAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Pending,
            Note = "Order placed",
            ChangedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        if (appliedCoupon is not null)
        {
            appliedCoupon.UsedCount++;
            await _unitOfWork.Coupons.UpdateAsync(appliedCoupon, cancellationToken);
        }

        await _unitOfWork.Carts.ClearCartItemsAsync(cart.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyOrderPlacedAsync(order.Id, userId, orderNumber, cancellationToken);
        await _activityTracking.TrackAsync(
            userId,
            ActivityType.Purchase,
            $"Placed order {orderNumber}",
            orderId: order.Id,
            cancellationToken: cancellationToken);

        var detail = await GetOrderDetailAsync(userId, order.Id, cancellationToken);

        return new PlaceOrderResponseDto
        {
            Success = true,
            Message = "Order placed successfully!",
            OrderId = order.Id,
            OrderNumber = orderNumber,
            Order = detail
        };
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(MapSummary).ToList();
    }

    public async Task<OrderDetailDto?> GetOrderDetailAsync(string userId, int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId, cancellationToken);
        if (order is null || order.UserId != userId)
            return null;

        return MapDetail(order);
    }

    public async Task<OrderDetailDto?> GetOrderDetailForAdminAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId, cancellationToken);
        return order is null ? null : MapDetail(order);
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> GetAllOrdersAsync(OrderStatus? status, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync(status, cancellationToken);
        return orders.Select(MapSummary).ToList();
    }

    public async Task<PlaceOrderResponseDto> UpdateOrderStatusAsync(
        string adminUserId,
        UpdateOrderStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Fail("Order not found.");

        if (order.Status == OrderStatus.Cancelled)
            return Fail("Cannot update a cancelled order.");

        if (order.Status == OrderStatus.Delivered)
            return Fail("Cannot update a delivered order.");

        var previousStatus = order.Status;
        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (request.Status == OrderStatus.Delivered && order.Payment is not null)
        {
            order.PaymentStatus = PaymentStatus.Completed;
            order.Payment.PaymentStatus = PaymentStatus.Completed;
            order.Payment.PaidAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Payment>().UpdateAsync(order.Payment, cancellationToken);
        }

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        var historyRepo = _unitOfWork.Repository<OrderStatusHistory>();
        await historyRepo.AddAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = request.Status,
            Note = request.Note,
            ChangedByUserId = adminUserId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (previousStatus != request.Status)
        {
            await _notificationService.NotifyOrderStatusChangeAsync(
                order.Id,
                order.UserId,
                request.Status,
                order.OrderNumber,
                cancellationToken);
        }

        var detail = await GetOrderDetailForAdminAsync(order.Id, cancellationToken);
        return new PlaceOrderResponseDto
        {
            Success = true,
            Message = $"Order status updated to {request.Status}.",
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Order = detail
        };
    }

    public async Task<PlaceOrderResponseDto> CancelOrderAsync(
        string userId,
        int orderId,
        string reason,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId, cancellationToken);
        if (order is null)
            return Fail("Order not found.");

        if (!isAdmin && order.UserId != userId)
            return Fail("Order not found.");

        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            return Fail("This order cannot be cancelled.");

        if (!isAdmin && order.Status is not OrderStatus.Pending and not OrderStatus.Processing)
            return Fail("Only pending or processing orders can be cancelled.");

        order.Status = OrderStatus.Cancelled;
        order.CancellationReason = reason.Trim();
        order.CancelledAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        if (order.PaymentStatus == PaymentStatus.Completed)
        {
            order.PaymentStatus = PaymentStatus.Refunded;
            if (order.Payment is not null)
            {
                order.Payment.PaymentStatus = PaymentStatus.Refunded;
                await _unitOfWork.Repository<Payment>().UpdateAsync(order.Payment, cancellationToken);
            }
        }

        foreach (var item in order.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
            {
                product.Stock += item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            }
        }

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        var historyRepo = _unitOfWork.Repository<OrderStatusHistory>();
        await historyRepo.AddAsync(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Cancelled,
            Note = reason.Trim(),
            ChangedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyOrderStatusChangeAsync(
            order.Id,
            order.UserId,
            OrderStatus.Cancelled,
            order.OrderNumber,
            cancellationToken);

        return new PlaceOrderResponseDto
        {
            Success = true,
            Message = "Order cancelled successfully.",
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        };
    }

    private OrderDetailDto MapDetail(Order order)
    {
        var dto = _mapper.Map<OrderDetailDto>(order);
        dto.Timeline = BuildTimeline(order);
        return dto;
    }

    private static OrderSummaryDto MapSummary(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        OrderDate = order.OrderDate,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        ItemCount = order.Items.Sum(i => i.Quantity)
    };

    private static List<OrderStatusTimelineItemDto> BuildTimeline(Order order)
    {
        var history = order.StatusHistory
            .OrderBy(h => h.CreatedAt)
            .ToList();

        var currentStatus = order.Status == OrderStatus.Cancelled
            ? OrderStatus.Cancelled
            : order.Status;

        if (order.Status == OrderStatus.Cancelled)
        {
            return
            [
                new OrderStatusTimelineItemDto
                {
                    Status = OrderStatus.Cancelled,
                    StatusLabel = "Cancelled",
                    ChangedAt = order.CancelledAt ?? order.UpdatedAt ?? order.CreatedAt,
                    Note = order.CancellationReason,
                    IsCompleted = true,
                    IsCurrent = true
                }
            ];
        }

        var statusOrder = TimelineStatuses
            .Select((status, index) => new { status, index })
            .ToDictionary(x => x.status, x => x.index);

        var currentIndex = statusOrder.GetValueOrDefault(currentStatus, 0);

        return TimelineStatuses.Select(status =>
        {
            var entry = history.LastOrDefault(h => h.Status == status);
            var index = statusOrder[status];
            return new OrderStatusTimelineItemDto
            {
                Status = status,
                StatusLabel = status.ToString(),
                ChangedAt = entry?.CreatedAt ?? default,
                Note = entry?.Note,
                IsCompleted = index <= currentIndex,
                IsCurrent = status == currentStatus
            };
        }).ToList();
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    private static PlaceOrderResponseDto Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}

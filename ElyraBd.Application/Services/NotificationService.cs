using AutoMapper;
using ElyraBd.Application.DTOs.Notifications;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId, 50, cancellationToken);
        return _mapper.Map<List<NotificationDto>>(notifications);
    }

    public Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default) =>
        _unitOfWork.Notifications.GetUnreadCountAsync(userId, cancellationToken);

    public async Task MarkAsReadAsync(int notificationId, string userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyOrderStatusChangeAsync(
        int orderId,
        string userId,
        OrderStatus status,
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        var (title, message) = status switch
        {
            OrderStatus.Processing => ("Order confirmed", $"Your order {orderNumber} is being processed."),
            OrderStatus.Packed => ("Order packed", $"Your order {orderNumber} has been packed and is ready to ship."),
            OrderStatus.Shipped => ("Order shipped", $"Your order {orderNumber} is on the way!"),
            OrderStatus.Delivered => ("Order delivered", $"Your order {orderNumber} has been delivered. Thank you for shopping!"),
            OrderStatus.Cancelled => ("Order cancelled", $"Your order {orderNumber} has been cancelled."),
            _ => ("Order update", $"Your order {orderNumber} status is now {status}.")
        };

        await CreateNotificationAsync(userId, title, message, orderId, cancellationToken);
    }

    public async Task NotifyOrderPlacedAsync(
        int orderId,
        string userId,
        string orderNumber,
        CancellationToken cancellationToken = default) =>
        await CreateNotificationAsync(
            userId,
            "Order placed",
            $"Your order {orderNumber} has been placed successfully. We will notify you when it ships.",
            orderId,
            cancellationToken);

    private async Task CreateNotificationAsync(
        string userId,
        string title,
        string message,
        int? orderId,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.Notifications.AddAsync(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            OrderId = orderId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

using ElyraBd.Application.DTOs.Notifications;
using ElyraBd.Core.Enums;

namespace ElyraBd.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int notificationId, string userId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
    Task NotifyOrderStatusChangeAsync(int orderId, string userId, OrderStatus status, string orderNumber, CancellationToken cancellationToken = default);
    Task NotifyOrderPlacedAsync(int orderId, string userId, string orderNumber, CancellationToken cancellationToken = default);
}

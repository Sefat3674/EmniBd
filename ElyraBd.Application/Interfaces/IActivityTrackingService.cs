using ElyraBd.Core.Enums;

namespace ElyraBd.Application.Interfaces;

public interface IActivityTrackingService
{
    Task TrackAsync(string userId, ActivityType type, string? description = null, int? productId = null, int? orderId = null, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task TrackProductViewAsync(string userId, int productId, CancellationToken cancellationToken = default);
}

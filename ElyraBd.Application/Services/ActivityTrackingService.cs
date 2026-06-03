using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Application.Services;

public class ActivityTrackingService : IActivityTrackingService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityTrackingService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task TrackAsync(
        string userId,
        ActivityType type,
        string? description = null,
        int? productId = null,
        int? orderId = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.UserActivities.AddAsync(new UserActivity
        {
            UserId = userId,
            ActivityType = type,
            Description = description,
            ProductId = productId,
            OrderId = orderId,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task TrackProductViewAsync(string userId, int productId, CancellationToken cancellationToken = default)
    {
        var historyRepo = _unitOfWork.Repository<ProductViewHistory>();
        var existing = (await historyRepo.FindAsync(
            h => h.UserId == userId && h.ProductId == productId,
            cancellationToken)).FirstOrDefault();

        if (existing is not null)
        {
            existing.ViewCount++;
            existing.ViewedAt = DateTime.UtcNow;
            await historyRepo.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            await historyRepo.AddAsync(new ProductViewHistory
            {
                UserId = userId,
                ProductId = productId,
                ViewedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        await TrackAsync(userId, ActivityType.ProductView, "Viewed product", productId, cancellationToken: cancellationToken);
    }
}

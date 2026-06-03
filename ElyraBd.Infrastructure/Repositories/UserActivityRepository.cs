using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class UserActivityRepository : GenericRepository<UserActivity>, IUserActivityRepository
{
    public UserActivityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<UserActivity>> GetRecentAsync(int count, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Product)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<int> CountByTypeTodayAsync(ActivityType type, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await DbSet.CountAsync(
            a => a.ActivityType == type && a.CreatedAt >= today,
            cancellationToken);
    }

    public async Task<IReadOnlyList<UserActivity>> GetByUserIdAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(a => a.Product)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
}

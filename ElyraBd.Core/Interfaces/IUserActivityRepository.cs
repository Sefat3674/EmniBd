using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Interfaces;

public interface IUserActivityRepository : IGenericRepository<UserActivity>
{
    Task<IReadOnlyList<UserActivity>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<int> CountByTypeTodayAsync(ActivityType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserActivity>> GetByUserIdAsync(string userId, int count, CancellationToken cancellationToken = default);
}

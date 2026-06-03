using ElyraBd.Application.DTOs.Dashboard;

namespace ElyraBd.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}

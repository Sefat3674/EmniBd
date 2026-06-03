using ElyraBd.Application.DTOs.Dashboard;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Enums;
using ElyraBd.Core.Interfaces;
namespace ElyraBd.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserStatsService _userStats;

    public DashboardService(IUnitOfWork unitOfWork, IUserStatsService userStats)
    {
        _unitOfWork = unitOfWork;
        _userStats = userStats;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(cancellationToken);
        var orderItems = await _unitOfWork.Repository<OrderItem>().GetAllAsync(cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalProducts = products.Count,
            TotalCustomers = await _userStats.GetCustomerCountAsync(cancellationToken),
            TotalOrders = orders.Count,
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            ActiveUsersToday = await _unitOfWork.UserActivities.CountByTypeTodayAsync(ActivityType.Login, cancellationToken),
            NewRegistrationsToday = await _userStats.GetNewRegistrationsTodayAsync(cancellationToken)
        };

        stats.OrdersByStatus = orders
            .GroupBy(o => o.Status)
            .Select(g => new ChartPointDto { Label = g.Key.ToString(), Value = g.Count() })
            .ToList();

        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        stats.ProductsByCategory = categories
            .Select(c => new ChartPointDto
            {
                Label = c.Name,
                Value = products.Count(p => p.CategoryId == c.Id)
            })
            .Where(x => x.Value > 0)
            .ToList();

        stats.TopSellingProducts = orderItems
            .GroupBy(i => i.ProductName)
            .Select(g => new TopProductDto
            {
                ProductName = g.Key,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(5)
            .ToList();

        var recent = await _unitOfWork.UserActivities.GetRecentAsync(10, cancellationToken);
        stats.RecentActivities = recent.Select(a => new RecentActivityDto
        {
            UserName = a.User.FullName,
            Activity = a.ActivityType.ToString(),
            ProductName = a.Product?.Name,
            CreatedAt = a.CreatedAt
        }).ToList();

        var now = DateTime.UtcNow;
        stats.SalesByMonth = Enumerable.Range(0, 6)
            .Select(i =>
            {
                var month = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                var next = month.AddMonths(1);
                return new ChartPointDto
                {
                    Label = month.ToString("MMM yyyy"),
                    Value = orders.Where(o => o.OrderDate >= month && o.OrderDate < next).Sum(o => o.TotalAmount)
                };
            })
            .Reverse()
            .ToList();

        return stats;
    }
}

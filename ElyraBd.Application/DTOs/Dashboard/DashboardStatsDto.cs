namespace ElyraBd.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ActiveUsersToday { get; set; }
    public int NewRegistrationsToday { get; set; }
    public IList<ChartPointDto> SalesByMonth { get; set; } = new List<ChartPointDto>();
    public IList<ChartPointDto> OrdersByStatus { get; set; } = new List<ChartPointDto>();
    public IList<ChartPointDto> ProductsByCategory { get; set; } = new List<ChartPointDto>();
    public IList<TopProductDto> TopSellingProducts { get; set; } = new List<TopProductDto>();
    public IList<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
}

public class ChartPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class TopProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class RecentActivityDto
{
    public string UserName { get; set; } = string.Empty;
    public string Activity { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public DateTime CreatedAt { get; set; }
}

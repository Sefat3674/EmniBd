namespace ElyraBd.Core.Interfaces;

public interface IUserStatsService
{
    Task<int> GetCustomerCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetNewRegistrationsTodayAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerSummary>> GetCustomersAsync(CancellationToken cancellationToken = default);
}

public class CustomerSummary
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}

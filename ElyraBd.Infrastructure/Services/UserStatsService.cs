using ElyraBd.Core.Constants;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Services;

public class UserStatsService : IUserStatsService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserStatsService(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<int> GetCustomerCountAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _userManager.GetUsersInRoleAsync(Roles.Customer);
        return customers.Count;
    }

    public async Task<int> GetNewRegistrationsTodayAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _userManager.Users.CountAsync(
            u => u.CreatedAt >= today && u.IsActive,
            cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerSummary>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _userManager.GetUsersInRoleAsync(Roles.Customer);
        return customers
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new CustomerSummary
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                IsActive = u.IsActive
            })
            .ToList();
    }
}

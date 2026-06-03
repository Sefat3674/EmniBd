using ElyraBd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElyraBd.Core.Constants;

namespace ElyraBd.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class CustomersController : Controller
{
    private readonly IUserStatsService _userStats;

    public CustomersController(IUserStatsService userStats) => _userStats = userStats;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var customers = await _userStats.GetCustomersAsync(cancellationToken);
        return View(customers);
    }
}

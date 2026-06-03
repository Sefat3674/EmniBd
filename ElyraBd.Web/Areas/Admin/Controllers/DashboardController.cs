using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetStatsAsync(cancellationToken);
        return View(stats);
    }
}

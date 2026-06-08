using ElyraBd.Application.DTOs.Orders;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Constants;
using ElyraBd.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ElyraBd.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    public async Task<IActionResult> Index(OrderStatus? status, CancellationToken cancellationToken)
    {
        ViewBag.Status = status;
        ViewBag.StatusList = Enum.GetValues<OrderStatus>()
            .Select(s => new SelectListItem(s.ToString(), s.ToString(), status == s))
            .Prepend(new SelectListItem("All statuses", ""))
            .ToList();

        var orders = await _orderService.GetAllOrdersAsync(status, cancellationToken);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderDetailForAdminAsync(id, cancellationToken);
        if (order is null) return NotFound();

        ViewBag.StatusList = Enum.GetValues<OrderStatus>()
            .Where(s => s != OrderStatus.Cancelled)
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(UpdateOrderStatusRequestDto model, CancellationToken cancellationToken)
    {
        var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var result = await _orderService.UpdateOrderStatusAsync(adminId, model, cancellationToken);

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id = model.OrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string reason, CancellationToken cancellationToken)
    {
        var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var result = await _orderService.CancelOrderAsync(adminId, id, reason, isAdmin: true, cancellationToken);

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }
}

using System.Security.Claims;
using ElyraBd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var order = await _orderService.GetOrderDetailAsync(userId, id, cancellationToken);
        if (order is null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var order = await _orderService.GetOrderDetailAsync(userId, id, cancellationToken);
        if (order is null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Invoice(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var order = await _orderService.GetOrderDetailAsync(userId, id, cancellationToken);
        if (order is null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Status(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var order = await _orderService.GetOrderDetailAsync(userId, id, cancellationToken);
        if (order is null) return NotFound();

        return Json(new
        {
            order.Status,
            statusLabel = order.Status.ToString(),
            order.Timeline
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string reason, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var result = await _orderService.CancelOrderAsync(userId, id, reason, isAdmin: false, cancellationToken);
        TempData[result.Success ? "OrderSuccess" : "OrderError"] = result.Message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private string? GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

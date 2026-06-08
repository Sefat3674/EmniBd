using System.Security.Claims;
using ElyraBd.Application.DTOs.Orders;
using ElyraBd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly IOrderService _orderService;

    public CheckoutController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    public async Task<IActionResult> Index(string? coupon, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var summary = await _orderService.GetCheckoutSummaryAsync(userId, coupon, cancellationToken);
        if (summary is null)
        {
            TempData["CartError"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.CouponCode = coupon;
        return View(summary);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(PlaceOrderRequestDto model, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var result = await _orderService.PlaceOrderAsync(userId, model, cancellationToken);
        if (!result.Success)
        {
            TempData["CheckoutError"] = result.Message;
            return RedirectToAction(nameof(Index), new { coupon = model.CouponCode });
        }

        return RedirectToAction("Confirmation", "Orders", new { id = result.OrderId });
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon([FromBody] string couponCode, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var summary = await _orderService.GetCheckoutSummaryAsync(userId, couponCode, cancellationToken);
        if (summary is null) return BadRequest(new { message = "Cart is empty." });

        return Ok(summary);
    }

    private string? GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

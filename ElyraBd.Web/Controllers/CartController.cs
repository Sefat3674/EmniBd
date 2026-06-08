using System.Security.Claims;
using ElyraBd.Application.DTOs.Carts;
using ElyraBd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) => _cartService = cartService;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Challenge();

        var cart = await _cartService.GetCartAsync(userId, cancellationToken);
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddToCartRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _cartService.AddToCartAsync(userId, request, cancellationToken);

        if (Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase))
            return result.Success ? Ok(result) : BadRequest(result);

        if (!result.Success)
        {
            TempData["CartError"] = result.Message;
            return RedirectToAction("Details", "Shop", new { id = request.ProductId });
        }

        TempData["CartSuccess"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddApi([FromBody] AddToCartRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _cartService.AddToCartAsync(userId, request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateCartItemRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _cartService.UpdateQuantityAsync(userId, request, cancellationToken);
        TempData[result.Success ? "CartSuccess" : "CartError"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _cartService.RemoveItemAsync(userId, cartItemId, cancellationToken);
        TempData[result.Success ? "CartSuccess" : "CartError"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private string? GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}

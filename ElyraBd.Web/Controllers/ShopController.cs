using ElyraBd.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Controllers;

public class ShopController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ShopController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string? q, int? categoryId, CancellationToken cancellationToken)
    {
        ViewBag.Categories = await _categoryService.GetActiveAsync(cancellationToken);
        ViewBag.SearchTerm = q;
        ViewBag.CategoryId = categoryId;

        var featured = await _productService.GetFeaturedAsync(8, cancellationToken);
        var onSale = await _productService.GetOnSaleAsync(8, cancellationToken);
        var products = await _productService.SearchAsync(q, categoryId, cancellationToken);

        ViewBag.Featured = featured;
        ViewBag.OnSale = onSale;
        return View(products);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;

        var product = await _productService.GetDetailForCustomerAsync(id, userId, cancellationToken);
        if (product is null || !product.IsActive) return NotFound();
        return View(product);
    }
}

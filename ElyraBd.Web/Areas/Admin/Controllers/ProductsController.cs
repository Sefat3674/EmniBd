using ElyraBd.Application.DTOs.Products;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElyraBd.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllForAdminAsync(cancellationToken);
        return View(products);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadCategoriesAsync(cancellationToken);
        return View(new ProductUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductUpsertDto model, IList<IFormFile>? images, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(cancellationToken);
            return View(model);
        }

        var streams = new List<Stream>();
        var names = new List<string>();
        if (images is { Count: > 0 })
        {
            foreach (var file in images.Where(f => f.Length > 0))
            {
                streams.Add(file.OpenReadStream());
                names.Add(file.FileName);
            }
        }

        await _productService.CreateAsync(model, streams, names, cancellationToken);
        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetDetailAsync(id, cancellationToken);
        if (product is null) return NotFound();

        await LoadCategoriesAsync(cancellationToken);
        return View(new ProductUpsertDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountPercent = product.DiscountPercent,
            Stock = product.Stock,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductUpsertDto model, IList<IFormFile>? images, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(cancellationToken);
            return View(model);
        }

        await _productService.UpdateAsync(model, cancellationToken);

        if (images is { Count: > 0 })
        {
            var streams = images.Where(f => f.Length > 0).Select(f => f.OpenReadStream()).ToList();
            var names = images.Where(f => f.Length > 0).Select(f => f.FileName).ToList();
            await _productService.AddImagesAsync(model.Id, streams, names, cancellationToken);
        }

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, CancellationToken cancellationToken)
    {
        await _productService.ToggleActiveAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
    }
}

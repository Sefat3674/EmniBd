using ElyraBd.Application.DTOs.Categories;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElyraBd.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return View(categories);
    }

    public IActionResult Create() => View(new CategoryDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        await _categoryService.CreateAsync(model, cancellationToken);
        TempData["Success"] = "Category created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        await _categoryService.UpdateAsync(model, cancellationToken);
        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _categoryService.DeleteAsync(id, cancellationToken);
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Index));
    }
}

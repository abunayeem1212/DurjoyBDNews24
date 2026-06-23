using DurjoyBDNews24.Admin.Filters;
using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.Admin.Controllers;

[AdminAuthFilter]
public class CategoryController(ICategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await categoryService.GetAllWithChildrenAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateCategoryDto());

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        await categoryService.CreateAsync(dto);
        TempData["Success"] = "বিভাগ তৈরি হয়েছে";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id);
        TempData["Success"] = "বিভাগ মুছে ফেলা হয়েছে";
        return RedirectToAction(nameof(Index));
    }
}
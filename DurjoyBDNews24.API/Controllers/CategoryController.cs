using DurjoyBDNews24.Application.DTOs.Category;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DurjoyBDNews24.API.Controllers;

public class CategoryController(ICategoryService categoryService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await categoryService.GetAllWithChildrenAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await categoryService.GetBySlugAsync(slug);
        if (result is null)
            return NotFound(ApiResponse<string>.Fail("বিভাগ পাওয়া যায়নি"));
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await categoryService.CreateAsync(dto);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "বিভাগ তৈরি হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin,Editor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "বিভাগ আপডেট হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await categoryService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("", "বিভাগ মুছে ফেলা হয়েছে"));
    }
}
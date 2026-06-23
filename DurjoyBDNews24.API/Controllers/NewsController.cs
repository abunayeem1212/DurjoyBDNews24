using DurjoyBDNews24.Application.DTOs.Article;
using DurjoyBDNews24.Application.DTOs.Common;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace DurjoyBDNews24.API.Controllers;

public class NewsController(IArticleService articleService, IUnitOfWork uow) : BaseController
{
    [HttpGet]
    [OutputCache(PolicyName = "home")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
    {
        var result = await articleService.GetPagedAsync(page, pageSize, categoryId, search);
        return Ok(ApiResponse<PagedResult<ArticleDto>>.Ok(result));
    }

    [HttpGet("{slug}")]
    [OutputCache(PolicyName = "article")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await articleService.GetBySlugAsync(slug);
        if (result is null)
            return NotFound(ApiResponse<string>.Fail("নিউজটি পাওয়া যায়নি"));
        return Ok(ApiResponse<ArticleDetailDto>.Ok(result));
    }

    [HttpGet("breaking")]
    public async Task<IActionResult> GetBreaking([FromQuery] int count = 10)
    {
        var result = await articleService.GetBreakingNewsAsync(count);
        return Ok(ApiResponse<IEnumerable<ArticleDto>>.Ok(result));
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 6)
    {
        var result = await articleService.GetFeaturedAsync(count);
        return Ok(ApiResponse<IEnumerable<ArticleDto>>.Ok(result));
    }

    [HttpGet("category/{slug}")]
    [OutputCache(PolicyName = "category")]
    public async Task<IActionResult> GetByCategory(
        string slug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await articleService.GetByCategoryAsync(slug, page, pageSize);
        return Ok(ApiResponse<IEnumerable<ArticleDto>>.Ok(result));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResponse<string>.Fail("সার্চ টার্ম দিন"));
        var result = await articleService.SearchAsync(q, page, pageSize);
        return Ok(ApiResponse<PagedResult<ArticleDto>>.Ok(result));
    }

    [Authorize(Roles = "Reporter,Editor,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleDto dto)
    {
        var result = await articleService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetBySlug),
            new { slug = result.Slug },
            ApiResponse<ArticleDetailDto>.Ok(result, "নিউজ তৈরি হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArticleDto dto)
    {
        dto.Id = id;
        var result = await articleService.UpdateAsync(dto, CurrentUserId);
        return Ok(ApiResponse<ArticleDetailDto>.Ok(result, "নিউজ আপডেট হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        await articleService.PublishAsync(id);
        return Ok(ApiResponse<string>.Ok("", "নিউজ প্রকাশিত হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(int id)
    {
        await articleService.UnpublishAsync(id);
        return Ok(ApiResponse<string>.Ok("", "নিউজ অপ্রকাশিত হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPatch("{id}/breaking")]
    public async Task<IActionResult> SetBreaking(int id, [FromQuery] bool value)
    {
        await articleService.SetBreakingAsync(id, value);
        return Ok(ApiResponse<string>.Ok("", "ব্রেকিং নিউজ আপডেট হয়েছে"));
    }

    [Authorize(Roles = "Editor,SuperAdmin")]
    [HttpPatch("{id}/featured")]
    public async Task<IActionResult> SetFeatured(int id, [FromQuery] bool value)
    {
        await articleService.SetFeaturedAsync(id, value);
        return Ok(ApiResponse<string>.Ok("", "ফিচার নিউজ আপডেট হয়েছে"));
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await articleService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("", "নিউজ মুছে ফেলা হয়েছে"));
    }

    [HttpPatch("{id}/share")]
    public async Task<IActionResult> IncrementShare(int id)
    {
        var article = await articleService.GetByIdAsync(id);
        if (article is null) return NotFound();

        await uow.Articles.IncrementShareCountAsync(id);
        return Ok(ApiResponse<string>.Ok("", ""));
    }
    //

}
using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.Interfaces;
using DurjoyBDNews24.Domain.Entities;
using DurjoyBDNews24.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DurjoyBDNews24.Application.Services;

public class CommentService(
    IUnitOfWork uow,
    IConfiguration config) : ICommentService
{
    //private bool AutoApprove =>
    //    config.GetValue<bool>("CommentSettings:AutoApprove");

    private async Task<bool> GetAutoApproveAsync()
    {
        var value = await uow.Settings.GetValueAsync("comment_auto_approve");
        return value == "true";
    }
    public async Task<IEnumerable<CommentDto>> GetByArticleAsync(int articleId)
    {
        var comments = await uow.Comments.GetByArticleIdAsync(articleId);
        return comments.Select(MapToDto);
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto, string userId)
    {
        var comment = new Comment
        {
            ArticleId = dto.ArticleId,
            Content = dto.Content,
            UserId = userId,
            ParentId = dto.ParentId,
            IsApproved = await GetAutoApproveAsync(),
            CreatedAt = DateTime.UtcNow
        };

        await uow.Comments.AddAsync(comment);
        await uow.SaveChangesAsync();

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task ApproveAsync(int id)
    {
        var comment = await uow.Comments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"মন্তব্য পাওয়া যায়নি: {id}");
        comment.IsApproved = true;
        await uow.Comments.UpdateAsync(comment);
        await uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var comment = await uow.Comments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"মন্তব্য পাওয়া যায়নি: {id}");
        comment.IsDeleted = true;
        await uow.Comments.UpdateAsync(comment);
        await uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentDto>> GetPendingAsync()
    {
        var comments = await uow.Comments.GetPendingAsync();
        return comments.Select(MapToDto);
    }

    private static CommentDto MapToDto(Comment c) => new()
    {
        Id = c.Id,
        Content = c.Content,
        UserName = !string.IsNullOrEmpty(c.User?.FullName)
        ? c.User.FullName
        : !string.IsNullOrEmpty(c.GuestName)
            ? c.GuestName
            : "পাঠক",
        UserNameBn = !string.IsNullOrEmpty(c.User?.FullNameBn)
        ? c.User.FullNameBn
        : !string.IsNullOrEmpty(c.GuestName)
            ? c.GuestName
            : "পাঠক",
        CreatedAt = c.CreatedAt,
        ParentId = c.ParentId,
        Replies = c.Replies?
        .Where(r => r.IsApproved && !r.IsDeleted)
        .Select(MapToDto)
        .ToList() ?? new()
    };

    public async Task<CommentDto> CreateGuestAsync(GuestCommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GuestName))
            throw new InvalidOperationException("নাম দিতে হবে");

        if (string.IsNullOrWhiteSpace(dto.Content) || dto.Content.Length < 5)
            throw new InvalidOperationException("কমপক্ষে ৫ অক্ষরের মন্তব্য লিখুন");

        var comment = new Comment
        {
            ArticleId = dto.ArticleId,
            Content = dto.Content,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            UserId = null,
            ParentId = dto.ParentId,
            //IsApproved = AutoApprove,
            IsApproved = await GetAutoApproveAsync(),
            CreatedAt = DateTime.UtcNow
        };

        await uow.Comments.AddAsync(comment);
        await uow.SaveChangesAsync();

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserName = comment.GuestName,
            UserNameBn = comment.GuestName,
            CreatedAt = comment.CreatedAt
        };
    }



}
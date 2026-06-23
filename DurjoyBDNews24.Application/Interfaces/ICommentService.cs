using DurjoyBDNews24.Application.DTOs.Comment;

namespace DurjoyBDNews24.Application.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByArticleAsync(int articleId);
    Task<CommentDto> CreateAsync(CreateCommentDto dto, string userId);
    Task<CommentDto> CreateGuestAsync(GuestCommentDto dto);
    Task ApproveAsync(int id);
    Task DeleteAsync(int id);
    Task<IEnumerable<CommentDto>> GetPendingAsync();
}
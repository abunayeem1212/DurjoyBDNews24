using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Application.DTOs.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserNameBn { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? ParentId { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}

public class CreateCommentDto
{
    public int ArticleId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}


public class GuestCommentDto
{
    public int ArticleId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string? GuestEmail { get; set; }
    public int? ParentId { get; set; }
}

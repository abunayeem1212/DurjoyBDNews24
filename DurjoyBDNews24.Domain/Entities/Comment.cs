using DurjoyBDNews24.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public int? ParentId { get; set; }


        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }

        public int ArticleId { get; set; }
        public string UserId { get; set; } = string.Empty;

        public Article Article { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public Comment? Parent { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}

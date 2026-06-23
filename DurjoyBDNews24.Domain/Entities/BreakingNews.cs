using DurjoyBDNews24.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Entities
{
    public class BreakingNews : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime? ExpiresAt { get; set; }
    }
}

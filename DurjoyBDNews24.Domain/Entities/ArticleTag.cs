using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Entities
{
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }

        public Article Article { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}

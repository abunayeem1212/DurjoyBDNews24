using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Entities
{
    public class MediaFile : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public string? AltTextBn { get; set; }
        public long FileSizeBytes { get; set; }
        public MediaType MediaType { get; set; }
        public int? ArticleId { get; set; }
        public string UploadedBy { get; set; } = string.Empty;

        public Article? Article { get; set; }
    }
}

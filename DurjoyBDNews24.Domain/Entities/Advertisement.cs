using DurjoyBDNews24.Domain.Common;
using DurjoyBDNews24.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Entities
{
    public class AdZone : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public AdPosition Position { get; set; }
        public string Size { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    }

    public class Advertisement : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public string AdvertiserName { get; set; } = string.Empty;
        public string? AdvertiserEmail { get; set; }
        public decimal DailyRate { get; set; }
        public int DailyCap { get; set; } = 10000;
        public long ImpressionCount { get; set; } = 0;
        public long ClickCount { get; set; } = 0;
        public bool IsApproved { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ZoneId { get; set; }
        public AdZone Zone { get; set; } = null!;
    }
}

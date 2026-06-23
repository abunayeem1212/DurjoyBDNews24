using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Domain.Enums;

public enum ArticleStatus
{
    Draft = 0,
    PendingReview = 1,
    Published = 2,
    Archived = 3,
    Rejected = 4
}

public enum MediaType
{
    Image = 0,
    Video = 1,
    Document = 2
}

public enum UserRole
{
    SuperAdmin = 0,
    Editor = 1,
    Reporter = 2,
    Reader = 3
}

public enum AdPosition
{
    HeaderBanner = 0,
    SidebarTop = 1,
    SidebarBottom = 2,
    InArticle = 3,
    FooterBanner = 4
}

public enum SubscriptionPlan
{
    Free = 0,
    Basic = 1,
    Premium = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Cancelled = 3,
    Refunded = 4
}

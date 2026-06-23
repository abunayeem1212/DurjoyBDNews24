using DurjoyBDNews24.Application.DTOs.Comment;
using DurjoyBDNews24.Application.DTOs.Payment;

namespace DurjoyBDNews24.Web.Models;

public class ProfileViewModel
{
    public string FullNameBn { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public SubscriptionDto? Subscription { get; set; }
    public IEnumerable<CommentDto> RecentComments { get; set; } = [];
    public bool IsPremium => Subscription?.IsActive == true;
}
using DurjoyBDNews24.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DurjoyBDNews24.API.Hubs;

public class NewsHub : Hub<INewsHubClient>
{
    public async Task JoinCategory(string categorySlug)
        => await Groups.AddToGroupAsync(Context.ConnectionId, categorySlug);

    public async Task LeaveCategory(string categorySlug)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, categorySlug);

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all-readers");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all-readers");
        await base.OnDisconnectedAsync(exception);
    }
}
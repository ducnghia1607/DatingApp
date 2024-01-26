using Microsoft.AspNetCore.SignalR;

namespace API;

public class PresenceHub : Hub
{
    private readonly PresenceTracker _presenceTracker;

    public PresenceHub(PresenceTracker presenceTracker)
    {
        _presenceTracker = presenceTracker;
    }
    public override async Task OnConnectedAsync()
    {
        var isOnline = await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
        var currentOnlineUsers = await _presenceTracker.GetUsersOnline();
        await Clients.Caller.SendAsync("GetUsersOnline", currentOnlineUsers);

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        var isOffline = await _presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOffline)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());
        }

        await base.OnDisconnectedAsync(exception);
    }
}

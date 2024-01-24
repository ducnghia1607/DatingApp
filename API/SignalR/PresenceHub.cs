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
        await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
        var currentOnlineUsers = await _presenceTracker.GetUsersOnline();
        await Clients.All.SendAsync("GetUsersOnline", currentOnlineUsers);

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        await _presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        var currentOnlineUsers = await _presenceTracker.GetUsersOnline();

        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());
        await Clients.All.SendAsync("GetUsersOnline", currentOnlineUsers);

        await base.OnDisconnectedAsync(exception);
    }
}

﻿using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API;

public class MessageHub : Hub
{

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public MessageHub(IUnitOfWork uow
    , IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        _uow = uow;
        _mapper = mapper;
        _presenceHub = presenceHub;
    }


    public async Task SendMessages(CreateMessageDto payload)
    {
        var username = Context.User.GetUserName();
        var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUsernameAsync(payload.RecipientUsername.ToLower());

        if (username == payload.RecipientUsername.ToLower())
            throw new HubException("You cannot send message to yourself");
        if (recipient == null) throw new HubException("Not found");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = payload.Content
        };
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _uow.MessageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            // if receiver is not in same group , send notification
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new
                {
                    username = sender.UserName,
                    knowAs = sender.KnownAs
                });
            }
        }

        _uow.MessageRepository.AddMessage(message);

        if (await _uow.Complete())
        {

            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }

    }


    public override async Task OnConnectedAsync()
    {
        // Get httpContext to access to HttpRequest get the other username through queryString
        var httpContext = Context.GetHttpContext();
        var other = httpContext.Request.Query["user"];

        var groupName = this.GetGroupName(Context.User.GetUserName(), other);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _uow.MessageRepository.GetMessagesThread(Context.User.GetUserName(), other);

        if (_uow.HasChanges()) await _uow.Complete();


        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string caller, string other)
    {
        var compareString = string.CompareOrdinal(caller, other) < 0;
        return compareString ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _uow.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
        if (group == null)
        {
            group = new Group(groupName);
            _uow.MessageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        if (await _uow.Complete())
        {
            return group;
        };
        throw new HubException("Failed to add to group");


    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _uow.MessageRepository.RemoveConnection(connection);
        if (await _uow.Complete()) return group;
        throw new HubException("Failed to remove from message group");
    }
}

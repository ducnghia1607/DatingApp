using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API;

public class MessageHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public async Task SendMessages(CreateMessageDto payload)
    {
        var username = Context.User.GetUserName();
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(payload.RecipientUsername.ToLower());

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

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
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

        var messages = await _messageRepository.GetMessagesThread(Context.User.GetUserName(), other);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        // await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    private string GetGroupName(string caller, string other)
    {
        var compareString = string.CompareOrdinal(caller, other) < 0;
        return compareString ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}

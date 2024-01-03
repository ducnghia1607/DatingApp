using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SentMessage(CreateMessageDto payload)
    {

        var username = User.GetUserName();
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(payload.RecipientUsername.ToLower());

        if (username == payload.RecipientUsername) return BadRequest("You cannot send messages to yourself");
        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = payload.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<PagedList<MessageDto>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        var username = User.GetUserName();
        messageParams.Username = username;
        var messages = await _messageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(new PaginationHeader(messageParams.pageNumber, messageParams.PageSize, messages.TotalCount, messages.TotalPages));
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
    {
        var currentUsername = User.GetUserName();

        var messages = await _messageRepository.GetMessagesThread(currentUsername, username);

        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var user = await _userRepository.GetUserByUsernameAsync(username);
        var message = await _messageRepository.GetMessage(id);
        if (message.RecipientUsername != username && message.SenderUsername != username) return Unauthorized();

        if (message.RecipientUsername == username) message.RecipientDeleted = true;
        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientDeleted && message.SenderDeleted) _messageRepository.RemoveMessage(message);

        if (await _messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to delete message");

    }
}

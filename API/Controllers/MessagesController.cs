using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MessagesController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SentMessage(CreateMessageDto payload)
    {

        var username = User.GetUserName();
        var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUsernameAsync(payload.RecipientUsername.ToLower());

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

        _uow.MessageRepository.AddMessage(message);

        if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
        MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();

        var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
            messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }



    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var user = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var message = await _uow.MessageRepository.GetMessage(id);
        if (message.RecipientUsername != username && message.SenderUsername != username) return Unauthorized();

        if (message.RecipientUsername == username) message.RecipientDeleted = true;
        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientDeleted && message.SenderDeleted) _uow.MessageRepository.RemoveMessage(message);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Failed to delete message");

    }
}

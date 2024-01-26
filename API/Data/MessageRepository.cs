
using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public void AddMessage(Message message)
    {

        _context.Messages.Add(message);
    }

    public void RemoveMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    // public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    // {
    //     // var query = _context.Messages.
    //     // OrderByDescending(x => x.DateSent).AsQueryable();

    //     var query = _context.Messages
    //       .OrderByDescending(x => x.DateSent)
    //       .AsQueryable();
    //     query = messageParams.Container switch
    //     {
    //         "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false),
    //         "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
    //         _ => query.Where(x => x.DateRead == null && x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false)
    //     };

    //     // var messages = _mapper.ProjectTo<MessageDto>(query);
    //     var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
    //     return await PagedList<MessageDto>.CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
    // }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages
            .OrderByDescending(x => x.DateSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username &&
             u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username &&
                u.SenderDeleted == false),
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username
                && u.RecipientDeleted == false && u.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Photos)
            .Include(m => m.Recipient).ThenInclude(u => u.Photos)
            .Where(m => m.SenderUsername == currentUsername && m.RecipientUsername == recipientUsername && m.SenderDeleted == false
            || m.SenderUsername == recipientUsername && m.RecipientUsername == currentUsername && m.RecipientDeleted == false
        ).OrderBy(m => m.DateSent).ToListAsync();

        // var thread = await messages
        var unreadMessages = messages.Where(
           m => m.DateRead == null && m.RecipientUsername == currentUsername
        ).ToList();

        if (unreadMessages.Any())
        {
            foreach (var m in unreadMessages)
            {
                m.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);

    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == groupName);

    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {

        return await _context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any(y => y.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }
}

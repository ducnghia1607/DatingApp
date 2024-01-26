﻿namespace API;

public interface IMessageRepository
{

    void AddMessage(Message message);

    void RemoveMessage(Message message);

    Task<Message> GetMessage(int id);

    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);


    void AddGroup(Group group);

    void RemoveConnection(Connection connection);

    Task<Connection> GetConnection(string connectionId);

    Task<Group> GetMessageGroup(string groupName);


    Task<Group> GetGroupForConnection(string connectionId);
}

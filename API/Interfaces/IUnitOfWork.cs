namespace API;

public interface IUnitOfWork
{
    IMessageRepository MessageRepository { get; }

    ILikesRepository LikeRepository { get; }

    IUserRepository UserRepository { get; }

    Task<bool> Complete();
    bool HasChanges();

}

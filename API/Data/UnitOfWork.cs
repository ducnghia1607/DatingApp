
using API.Data;
using AutoMapper;

namespace API;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UnitOfWork(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public ILikesRepository LikeRepository => new LikesRepository(_context);

    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}

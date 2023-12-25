using API.Data;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberByUsernameAsync(string username)
    {
        return await _context.Users
                .Where(user => user.Username == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await _context.Users
                 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync<AppUser>(x => x.Username == username);
    }


    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users.Include(x => x.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    // And this just tells our Entity Framework Tracker that something has changed with the entity, the user
    // that we've passed in here, and we're not saving anything from this method at this point.
    // We're just informing the Entity Framework Tracker that an entity has been updated.
    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}

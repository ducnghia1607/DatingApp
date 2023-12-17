using API.Entities;

namespace API;

public interface IUserRepository
{

    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByIdUsernameAsync(string username);

    Task<IEnumerable<MemberDto>> GetMembersAsync();

    Task<MemberDto> GetMemberByUsernameAsync(string username);
    Task<bool> SaveAllAsync();

    void Update(AppUser user);


}

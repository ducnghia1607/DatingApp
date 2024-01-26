using API.Entities;

namespace API;

public interface IUserRepository
{

    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);

    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

    Task<MemberDto> GetMemberByUsernameAsync(string username);

    void Update(AppUser user);

    Task<string> GetUserGender(string username);
}

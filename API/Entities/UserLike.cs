using API.Entities;

namespace API;

public class UserLike
{
    public int SourceUserId { get; set; }
    public AppUser SourceUser { get; set; }
    public AppUser TargetUser { get; set; }
    public int TargetUserId { get; set; }

}

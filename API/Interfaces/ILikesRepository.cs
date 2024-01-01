using API.Entities;

namespace API;

public interface ILikesRepository
{
    // Get 1 element in table UserLike with primary key 
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);


    // Get user with List LikedUser
    // check to see if a user already has been liked by another user.
    Task<AppUser> GetUserWithLikes(int userId);


    //// Get 1 User with  field LikedUsers or  LikedByUsers
    Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);

}

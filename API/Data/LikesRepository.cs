using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
    {
        // Do dang o trong repository nen khong thi dung User.GetUserId() nen moi phai truyen vao thong qua parameter 
        var users = _context.Users.AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (likeParams.Predicate == "liked")
        {
            likes = likes.Where(x => x.SourceUserId == likeParams.UserId);
            users = likes.Select(x => x.TargetUser);
        }
        if (likeParams.Predicate == "likedBy")
        {
            likes = likes.Where(x => x.TargetUserId == likeParams.UserId);
            users = likes.Select(x => x.SourceUser);
        }

        var likedUser = users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            Age = user.DateOfBirth.CalculateAge(),
            KnownAs = user.KnownAs,
            City = user.City,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.isMain).Url,
            Id = user.Id

        });

        return await PagedList<LikeDto>.CreateAsync(likedUser, likeParams.pageNumber, likeParams.PageSize);
    }

    // Get user with likedUserList 
    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
        .Include(x => x.LikedUsers)
        .FirstOrDefaultAsync(x => x.Id == userId);
    }
}

using API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var user = await _userRepository.GetUserByIdAsync(sourceUserId);
        var LikedUser = await _userRepository.GetUserByUsernameAsync(username);

        if (LikedUser == null) return NotFound();

        if (user.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, LikedUser.Id);
        if (userLike != null)
            return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = LikedUser.Id
        };

        user.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Something went wrong");

    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
    {
        var userId = User.GetUserId();
        likeParams.UserId = userId;
        var userLikes = await _likesRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(new PaginationHeader(likeParams.pageNumber, likeParams.PageSize, userLikes.TotalCount, userLikes.TotalPages));

        return Ok(userLikes);
    }
}

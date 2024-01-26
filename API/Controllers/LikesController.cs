using API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class LikesController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public LikesController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var user = await _uow.UserRepository.GetUserByIdAsync(sourceUserId);
        var LikedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);

        if (LikedUser == null) return NotFound();

        if (user.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _uow.LikeRepository.GetUserLike(sourceUserId, LikedUser.Id);
        if (userLike != null)
            return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = LikedUser.Id
        };

        user.LikedUsers.Add(userLike);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Something went wrong");

    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
    {
        var userId = User.GetUserId();
        likeParams.UserId = userId;
        var userLikes = await _uow.LikeRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(new PaginationHeader(likeParams.pageNumber, likeParams.PageSize, userLikes.TotalCount, userLikes.TotalPages));

        return Ok(userLikes);
    }
}

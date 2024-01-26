
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
    {
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _uow.UserRepository.GetUserGender(User.GetUserName());
        var username = User.GetUserName();
        userParams.CurrentUsername = username;
        // return Ok(await _uow.UserRepository.GetMembersAsync());

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "female" ? "male" : "female";
        }


        var users = await _uow.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    }


    [HttpGet("{username}")]  // api /users/2
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {

        return await _uow.UserRepository.GetMemberByUsernameAsync(username);
    }

    [HttpPut]

    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // var username2 = 
        // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.GetUserName();
        var user = await _uow.UserRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();
        _mapper.Map(memberUpdateDto, user);
        _uow.UserRepository.Update(user);

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]  // api /users/2
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var username = User.GetUserName();
        var user = await _uow.UserRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var result = _photoService.AddImageAsync(file);
        if (result.Result.Error != null) return BadRequest(result.Result.Error.Message);

        var photo = new Photo
        {
            Url = result.Result.SecureUrl.AbsoluteUri,
            PublicId = result.Result.PublicId,
        };
        if (user.Photos.Count == 0) photo.isMain = true;
        // EF tracks that user
        user.Photos.Add(photo);
        if (await _uow.Complete())
        {
            var actionName = nameof(GetUser);
            var createdResource = _mapper.Map<PhotoDto>(photo); // new created data 
            var routeValues = new { username = user.UserName }; // api/users/{username} link assign to location header
            return CreatedAtAction(actionName, routeValues, createdResource);
        }

        return BadRequest("Adding photo failed");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var username = User.GetUserName();
        var user = await _uow.UserRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.isMain) return BadRequest("This photo is already main photo");


        var currentMainPhoto = user.Photos.FirstOrDefault(x => x.isMain);

        if (currentMainPhoto != null) currentMainPhoto.isMain = false;

        photo.isMain = true;
        if (await _uow.Complete()) return NoContent();
        return BadRequest("Problem adding main photo");
    }

    [HttpDelete("delete-photo/{photoID}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.isMain) return BadRequest("Can't delete the main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeleteImageAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting the  photo");

    }
}



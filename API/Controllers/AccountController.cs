
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]  // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto account)
    {
        if (!await UserExists(account.Username)) return BadRequest("Username already exists");

        var user = _mapper.Map<AppUser>(account);
        user.UserName = account.Username.ToLower();

        var result = await _userManager.CreateAsync(user, account.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);
        var resultRole = await _userManager.AddToRoleAsync(user, "Member");
        if (!resultRole.Succeeded) return BadRequest(resultRole.Errors);
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            // PhotoUrl = user.Photos.FirstOrDefault(p => p.isMain).Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto account)
    {
        var user = await _userManager.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(user => user.UserName == account.Username);

        //     var user = await _userManager.Users
        //  .SingleOrDefaultAsync(user => user.UserName == account.Username);
        // var user = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(u => u.UserName == account.Username);
        // var user = await _userManager.FindByNameAsync(account.Username);

        if (user == null) return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, account.Password);
        if (!result) return Unauthorized();

        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

    }

    public async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AllAsync(user => user.UserName != username.ToLower());
    }
}

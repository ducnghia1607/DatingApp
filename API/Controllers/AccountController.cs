using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]  // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto account)
    {
        if (!await UserExists(account.Username)) return BadRequest("Username already exists");
        using var hmac = new HMACSHA512();

        var user = _mapper.Map<AppUser>(account);

        user.Username = account.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(account.Password));
        user.PasswordSalt = hmac.Key;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.Username,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto account)
    {
        var user = await _context.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(user => user.Username == account.Username);
        if (user == null) return Unauthorized();

        //Initializes a new instance of the HMACSHA512 class with the specified key data.
        // Tạo ra cùng thuật toán Hashpassword bằng cách truyền vào specified key 
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(account.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized();
        }

        return new UserDto
        {
            Username = user.Username,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url
        };

    }

    public async Task<bool> UserExists(string username)
    {
        return await _context.Users.AllAsync(user => user.Username != username.ToLower());
    }
}

﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }

    //TokenService 
    string ITokenService.CreateToken(AppUser user)
    {
        // Create list claims contain information 
        var claims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.NameId,user.UserName),
        };

        //create Credentials 
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // Contains some information which used to create a security token.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        // A SecurityTokenHandler designed for creating and validating Json Web Tokens. See:
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // return Json Web Tokens contain three part Header, Payload(claims,credentials),Signature 
        return tokenHandler.WriteToken(token);
    }
}

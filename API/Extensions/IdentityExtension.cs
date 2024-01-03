using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API;

public static class IdentityExtension
{

    public static IServiceCollection AddIdentityServices(this IServiceCollection service, IConfiguration config)
    {

        service.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>();


        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
    {
        // Gets or sets the parameters used to validate identity tokens.
        option.TokenValidationParameters = new TokenValidationParameters
        {
            //Gets or sets a boolean to control if the issuer will be validated during token validation.
            ValidateIssuer = false,
            //Gets or sets a boolean to control if the lifetime will be validated during token validation.
            ValidateLifetime = true,
            // Validate signature
            ValidateIssuerSigningKey = true,
            // Gets or sets a boolean to control if the token replay will be validated during token validation.
            ValidateTokenReplay = false,

            ValidateAudience = false,
            //We need to specify the secret key to be used for signature validation.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]))

        };
    });

        service.AddAuthorization(
            opt =>
            {
                opt.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("RequiredModeratorOrAdminRole", policy => policy.RequireRole("Admin", "Moderator"));
            }
        );

        return service;
    }

}

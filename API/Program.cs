using System.Text;
using API;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddApplicationServicers(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();
app.UseCors((builder) =>
{
    builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
});
app.UseHttpsRedirection();

// Authentication 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

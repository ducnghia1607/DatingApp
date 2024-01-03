using API;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddApplicationServicers(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors((builder) =>
{
    builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
});
app.UseHttpsRedirection();

// Authentication 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;
try
{
    var context = service.GetRequiredService<DataContext>();
    // Reset database
    var userManager = service.GetRequiredService<UserManager<AppUser>>();
    var roleManager = service.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = service.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migrations");
}

app.Run();

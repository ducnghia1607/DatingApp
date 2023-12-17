using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServicers(this IServiceCollection service, IConfiguration config)
    {
        service.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
});
        service.AddCors();

        service.AddScoped<ITokenService, TokenService>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return service;
    }
}

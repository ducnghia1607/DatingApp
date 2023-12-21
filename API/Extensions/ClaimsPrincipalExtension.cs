using System.Security.Claims;

namespace API;

public static class ClaimsPrincipleExtension
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

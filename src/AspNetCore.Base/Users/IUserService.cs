using System.Security.Claims;

namespace AspNetCore.Base.Users
{
    public interface IUserService
    {
        ClaimsPrincipal User { get; }
        string UserId { get; }
        string UserName { get; }
    }
}

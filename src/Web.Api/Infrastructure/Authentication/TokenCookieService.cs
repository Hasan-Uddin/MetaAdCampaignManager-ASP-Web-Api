
namespace Web.Api.Infrastructure.Authentication;

public sealed class TokenCookieService : ITokenCookieService
{
    public void SetAccessToken(HttpContext context, string token, DateTime expiresAtUtc)
    {
        context.Response.Cookies.Append(
            "access_token",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAtUtc
            });
    }
}

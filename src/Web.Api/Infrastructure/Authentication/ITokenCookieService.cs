namespace Web.Api.Infrastructure.Authentication;

public interface ITokenCookieService
{
    void SetAccessToken(HttpContext context, string token, DateTime expiresAtUtc);
}

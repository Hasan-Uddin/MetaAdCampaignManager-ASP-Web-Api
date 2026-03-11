using System.Globalization;
using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Domain.Users;
using Microsoft.IdentityModel.JsonWebTokens;
using SharedKernel;
using Web.Api.Infrastructure.Authentication;

namespace Web.Api.Infrastructure;

public class JwtSlidingMiddleware
{
    private readonly RequestDelegate _next;
    public JwtSlidingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(
        HttpContext context,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider,
        IUserRepository userRepository,
        ITokenCookieService cookieService)
    {
        string? token = context.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JsonWebTokenHandler();
            JsonWebToken jwt = handler.ReadJsonWebToken(token);

            var userId = Guid.Parse(jwt.GetClaim(JwtRegisteredClaimNames.Sub)!.Value);

            DateTime expiresAt = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(jwt.GetClaim(JwtRegisteredClaimNames.Exp)!.Value, CultureInfo.InvariantCulture)
            ).UtcDateTime;

            long expiresInSec = (long)(expiresAt - dateTimeProvider.UtcNow).TotalSeconds;
            var tokenResult = new TokenResponse(token, expiresInSec, expiresAt);

            if (tokenProvider.ShouldSlide(tokenResult))
            {
                User? user = await userRepository.GetByIdAsync(userId);
                if (user is not null)
                {
                    TokenResponse newToken = tokenProvider.Create(user);
                    cookieService.SetAccessToken(context, newToken.AccessToken, newToken.ExpiresAtUtc);
                }
            }
        }

        await _next(context);
    }
}

using System.Security.Claims;
using System.Text;
using Application.Abstractions.Authentication;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Authentication;

internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public TokenResponse Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        int expirationMinutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        long expiresInSec = (long)(expiresAt - DateTime.UtcNow).TotalSeconds;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("name", user.Name),
                new Claim("picture", user.PictureUrl ?? string.Empty),
                new Claim("facebook_id", user.FacebookId ?? string.Empty)
            ]),
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return new TokenResponse(token, expiresInSec, expiresAt);
    }

    public bool ShouldSlide(TokenResponse tokenResult)
    {
        int threshold = configuration.GetValue<int>("Jwt:SlidingThresholdInMinutes");
        return tokenResult.ExpiresAtUtc - DateTime.UtcNow <= TimeSpan.FromMinutes(threshold);
    }
}

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
    public TokenResult Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        int expirationMinutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return new TokenResult(token, expiresAt);
    }

    public bool ShouldSlide(TokenResult tokenResult)
    {
        int threshold = configuration.GetValue<int>("Jwt:SlidingThresholdInMinutes");
        return tokenResult.ExpiresAtUtc - DateTime.UtcNow <= TimeSpan.FromMinutes(threshold);
    }
}

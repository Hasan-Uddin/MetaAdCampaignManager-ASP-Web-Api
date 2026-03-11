using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    TokenResponse Create(User user);
    bool ShouldSlide(TokenResponse tokenResult);
}

public sealed record TokenResponse(
    string AccessToken,
    long ExpiresInSec,
    DateTime ExpiresAtUtc);

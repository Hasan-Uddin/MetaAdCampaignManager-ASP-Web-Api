using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    TokenResult Create(User user);
    bool ShouldSlide(TokenResult tokenResult);
}

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);

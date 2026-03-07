using Google.Apis.Auth.OAuth2;

namespace Application.Abstractions.Authentication;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ExchangeCodeAsync(string code);
    Task<UserCredential> CreateUserCredentialAsync(
        string refreshToken,
        CancellationToken cancellationToken);
}

public class GoogleUserInfo
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string PictureUrl { get; set; }
    public string GoogleId { get; set; }
    public string? GoogleRefreshToken { get; set; }
    public int? ExpiresIn { get; init; }
}

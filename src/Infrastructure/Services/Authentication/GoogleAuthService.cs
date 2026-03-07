using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Authentication;

public sealed class GoogleAuthService : IGoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public GoogleAuthService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    // Exchange authorization code
    public async Task<GoogleUserInfo> ExchangeCodeAsync(string code)
    {
        TokenResponseDto tokenResponse = await RequestTokenAsync(code);

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = handler.ReadJwtToken(tokenResponse.IdToken);

        return new GoogleUserInfo
        {
            Email = jwt.Claims.First(x => x.Type == "email").Value,
            Name = jwt.Claims.First(x => x.Type == "name").Value,
            GoogleId = jwt.Claims.First(x => x.Type == "sub").Value,
            PictureUrl = jwt.Claims.FirstOrDefault(x => x.Type == "picture")?.Value ?? string.Empty,

            GoogleRefreshToken = tokenResponse.RefreshToken
        };
    }

    // Create credential (AUTO REFRESH ENABLED)
    public async Task<UserCredential> CreateUserCredentialAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        using var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _configuration["Google:ClientId"],
                    ClientSecret = _configuration["Google:ClientSecret"]
                },
                Scopes = new[] { CalendarService.Scope.Calendar }
            });

        var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
        {
            RefreshToken = refreshToken
        };

        var credential = new UserCredential(flow, "user", token);

        // Ensures access token is valid immediately
        await credential.RefreshTokenAsync(cancellationToken);

        return credential;
    }

    // Exchange code for tokens
    private async Task<TokenResponseDto> RequestTokenAsync(string code)
    {
        const string endpoint = "https://oauth2.googleapis.com/token";

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _configuration["Google:ClientId"]! },
            { "client_secret", _configuration["Google:ClientSecret"]! },
            { "redirect_uri", _configuration["Google:RedirectUri"]! },
            { "grant_type", "authorization_code" }
        });

        HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Google token exchange failed: {error}");
        }

        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TokenResponseDto>(
            json, JsonOptions)!;
    }

    private sealed class TokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = default!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;
    }
}

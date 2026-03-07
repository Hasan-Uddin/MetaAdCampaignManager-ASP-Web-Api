using Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Authentication;

public class GoogleAuthSettings : IGoogleAuthSettings
{
    public string ClientId { get; }
    public string RedirectUri { get; }

    public GoogleAuthSettings(IConfiguration configuration)
    {
        ClientId = configuration["Google:ClientId"] ?? "";
        RedirectUri = configuration["Google:RedirectUri"] ?? "";
    }
}

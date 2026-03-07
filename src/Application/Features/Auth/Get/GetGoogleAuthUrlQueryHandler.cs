using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Auth.Get;

public sealed class GetGoogleAuthUrlQueryHandler(
    IGoogleAuthSettings settings) : IQueryHandler<GetGoogleAuthUrlQuery, GoogleAuthUrlQueryResponse>
{
    async Task<Result<GoogleAuthUrlQueryResponse>> IQueryHandler<GetGoogleAuthUrlQuery, GoogleAuthUrlQueryResponse>.Handle(GetGoogleAuthUrlQuery query, CancellationToken cancellationToken)
    {
        string scope = "openid email profile https://www.googleapis.com/auth/calendar";

        string url = $"https://accounts.google.com/o/oauth2/v2/auth" +
                  $"?client_id={settings.ClientId}" +
                  $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                  $"&response_type=code" +
                  $"&scope={Uri.EscapeDataString(scope)}" +
                  $"&access_type=offline" +
                  $"&prompt=consent";

        var response = new GoogleAuthUrlQueryResponse(url);

        return Result.Success(response);
    }
}

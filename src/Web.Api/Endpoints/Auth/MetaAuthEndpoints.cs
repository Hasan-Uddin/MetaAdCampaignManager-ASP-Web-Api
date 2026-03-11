using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Messaging;
using Application.Features.Auth.MetaOAuth;
using SharedKernel;
using Web.Api.Infrastructure.Authentication;

namespace Web.Api.Endpoints.Auth;

public sealed class MetaAuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Step 1 — redirect to Meta login
        app.MapGet("meta/auth/login", (
            IMetaAuthService metaAuth,
            IConfiguration config) =>
        {
            string redirectUri = $"{config["App:BaseUrl"]}/meta/auth/callback";  // ← use Api:BaseUrl consistently
            string state = Guid.NewGuid().ToString();
            Uri loginUrl = metaAuth.GetLoginUrl(redirectUri, state);
            return Results.Redirect(loginUrl.AbsoluteUri);
        }).WithTags(Tags.Auth);

        app.MapGet("meta/auth/callback", async (
            string code,
            HttpContext httpContext,
            IDateTimeProvider dateTimeProvider,
            ICommandHandler<MetaOAuthCallbackCommand, MetaOAuthCallbackCommandResponse> handler,
            ITokenCookieService cookieService,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            string redirectUri = $"{configuration["App:BaseUrl"]}/meta/auth/callback";  // ← same value

            Result<MetaOAuthCallbackCommandResponse> result = await handler.Handle(
                new MetaOAuthCallbackCommand(code, redirectUri), ct);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            TokenResponse tokenResult = result.Value.TokenResult;
            cookieService.SetAccessToken(httpContext, tokenResult.AccessToken, tokenResult.ExpiresAtUtc);

            return Results.Redirect(configuration["Frontend:BaseUrl"]!);
        }).WithTags(Tags.Auth);
    }
}

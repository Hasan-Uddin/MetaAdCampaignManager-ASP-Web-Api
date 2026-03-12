using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Messaging;
using Application.Features.Auth.MetaOAuth;
using Infrastructure.Services.Meta;
using Microsoft.Extensions.Options;
using SharedKernel;
using Web.Api.Infrastructure.Authentication;

namespace Web.Api.Endpoints.Auth;

public sealed class MetaAuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/auth/login", (
            IMetaAuthService metaAuth,
            IOptions<MetaApiOptions> options,
            IConfiguration config) =>
        {
            string redirectUri = $"{config["App:BaseUrl"]}/meta/auth/callback";
            string state = Guid.NewGuid().ToString();
            // Use bootstrap AppId from options for the very first login
            Uri loginUrl = metaAuth.GetLoginUrl(options.Value.AppID, redirectUri, state);
            return Results.Redirect(loginUrl.AbsoluteUri);
        }).WithTags(Tags.Auth);

        app.MapGet("meta/auth/callback", async (
            string code,
            HttpContext httpContext,
            IDateTimeProvider dateTimeProvider,
            ICommandHandler<MetaOAuthCallbackCommand, MetaOAuthCallbackCommandResponse> handler,
            ITokenCookieService cookieService,
            IOptions<MetaApiOptions> options,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            string redirectUri = $"{configuration["App:BaseUrl"]}/meta/auth/callback";

            Result<MetaOAuthCallbackCommandResponse> result = await handler.Handle(
                new MetaOAuthCallbackCommand(
                    code,
                    redirectUri,
                    options.Value.AppID,
                    options.Value.AppSecret), ct);

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


//### The full flow
//```
//1. GET /meta/auth/login
//      → uses AppID from MetaApiOptions(bootstrap only)
//      → redirects to Meta login

//2.Meta callback → saves AppId +AppSecret from options into DB

//3.GET / meta / settings / apps          ← new
//      → returns list of all Meta apps the user manages
//      → user picks the right AppId

//4. PUT /meta/settings/app-credentials  { appId, appSecret }   ← new
//      → overrides the DB with the chosen app credentials
//      → all future API calls use these credentials

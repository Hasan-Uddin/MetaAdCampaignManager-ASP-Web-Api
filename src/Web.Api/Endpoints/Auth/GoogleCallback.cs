using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Features.Auth.Login;
using SharedKernel;
using Web.Api.Infrastructure.Authentication;

namespace Web.Api.Endpoints.Auth;

public class GoogleCallback : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/auth/google/callback",
                async (
                    string? code,
                    IConfiguration configuration,
                    ICommandHandler<GoogleLoginCommand, GoogleLoginCommandResponse> handler,
                    HttpContext httpContext,
                    ITokenCookieService cookieService,
                    CancellationToken ct) =>
                {
                    var command = new GoogleLoginCommand(code ?? string.Empty);
                    Result<GoogleLoginCommandResponse> result = await handler.Handle(command, ct);
            
                    if (result.IsFailure)
                    {
                        return Results.BadRequest(result.Error);
                    }

                    TokenResult tokenResult = result.Value.tokenResult;

                    cookieService.SetAccessToken(
                        httpContext,
                        tokenResult.AccessToken,
                        tokenResult.ExpiresAtUtc);

                    string? frontendBase = configuration["Frontend:BaseUrl"];

                    return Results.Redirect(frontendBase!);
                }
            )
            .WithTags(Tags.Auth);
    }
}

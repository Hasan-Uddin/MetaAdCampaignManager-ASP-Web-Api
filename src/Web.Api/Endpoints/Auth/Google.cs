using Application.Abstractions.Messaging;
using Application.Features.Auth.Get;
using SharedKernel;

namespace Web.Api.Endpoints.Auth;

internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/google", 
            async (
                IQueryHandler<GetGoogleAuthUrlQuery, GoogleAuthUrlQueryResponse> handler,
                CancellationToken ct) =>
            {
                var query = new GetGoogleAuthUrlQuery();
                Result<GoogleAuthUrlQueryResponse> result = await handler.Handle(query, ct);
                return Results.Redirect(result.Value.Url);
            }
        ).WithTags(Tags.Auth);
    }
}

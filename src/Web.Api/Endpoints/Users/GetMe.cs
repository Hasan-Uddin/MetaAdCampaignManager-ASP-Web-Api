using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Features.Users.GetMe;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class GetMe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users/me", async (
            IUserContext userContext,
            IQueryHandler<GetMeQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            if(!userContext.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            var query = new GetMeQuery();

            Result<UserResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}

using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Form;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/pages/{pageId}/forms", async (
            string pageId,
            IQueryHandler<GetFormsQuery, List<FormResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<FormResponse>> result =
                await handler.Handle(new GetFormsQuery(pageId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Forms)
        .Produces<List<FormResponse>>();
    }
}

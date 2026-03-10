using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Form;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/forms/{formId}", async (
            string formId,
            IQueryHandler<GetFormByIdQuery, FormResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<FormResponse> result =
                await handler.Handle(new GetFormByIdQuery(formId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Forms)
        .Produces<FormResponse>();
    }
}

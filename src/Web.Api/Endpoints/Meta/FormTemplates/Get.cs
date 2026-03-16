using Application.Abstractions.Messaging;
using Application.Features.Meta.FormTemplates;
using Application.Features.Meta.FormTemplates.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.FormTemplates;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/form-templates", async (
            IQueryHandler<GetFormTemplatesQuery, List<FormTemplateResponse>> handler,
            CancellationToken ct) =>
        {
            Result<List<FormTemplateResponse>> result = await handler.Handle(new GetFormTemplatesQuery(), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.FormTemplates);
    }
}

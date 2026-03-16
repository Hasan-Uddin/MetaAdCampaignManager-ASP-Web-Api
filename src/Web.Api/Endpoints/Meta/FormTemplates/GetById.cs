using Application.Abstractions.Messaging;
using Application.Features.Meta.FormTemplates;
using Application.Features.Meta.FormTemplates.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.FormTemplates;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/form-templates/{templateId:guid}", async (
            Guid templateId,
            IQueryHandler<GetFormTemplateByIdQuery, FormTemplateResponse> handler,
            CancellationToken ct) =>
        {
            Result<FormTemplateResponse> result = await handler.Handle(new GetFormTemplateByIdQuery(templateId), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.FormTemplates);
    }
}

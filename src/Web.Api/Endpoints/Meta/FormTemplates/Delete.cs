using Application.Abstractions.Messaging;
using Application.Features.Meta.FormTemplates.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.FormTemplates;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("meta/form-templates/{templateId:guid}", async (
            Guid templateId,
            ICommandHandler<DeleteFormTemplateCommand> handler,
            CancellationToken ct) =>
        {
            Result result = await handler.Handle(new DeleteFormTemplateCommand(templateId), ct);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.FormTemplates);
    }
}

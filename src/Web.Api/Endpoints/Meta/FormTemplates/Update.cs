using Application.Abstractions.Messaging;
using Application.Features.Meta.FormTemplates.Create;
using Application.Features.Meta.FormTemplates.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.FormTemplates;

internal sealed class Update : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("meta/form-templates/{templateId:guid}", async (
            Guid templateId,
            CreateFormTemplateRequest request,
            ICommandHandler<UpdateFormTemplateCommand> handler,
            CancellationToken ct) =>
        {
            var command = new UpdateFormTemplateCommand(
                templateId,
                request.Name,
                request.Description,
                request.Questions,
                request.IsDefault);

            Result result = await handler.Handle(command, ct);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.FormTemplates);
    }
}

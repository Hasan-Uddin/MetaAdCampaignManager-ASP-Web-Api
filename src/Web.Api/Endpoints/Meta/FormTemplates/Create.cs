using Application.Abstractions.Messaging;
using Application.Features.Meta.FormTemplates.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.FormTemplates;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meta/form-templates", async (
            CreateFormTemplateRequest request,
            ICommandHandler<CreateFormTemplateCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var command = new CreateFormTemplateCommand(
                request.Name,
                request.Description,
                request.Questions,
                request.IsDefault);

            Result<Guid> result = await handler.Handle(command, ct);
            return result.Match(id => Results.Created($"meta/templates/{id}", id), CustomResults.Problem);
        }).WithTags(Tags.FormTemplates);
    }
}

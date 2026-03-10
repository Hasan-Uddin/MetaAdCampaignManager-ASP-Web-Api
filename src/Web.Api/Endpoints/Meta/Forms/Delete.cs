using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Form;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("meta/forms/{formId}", async (
            string formId,
            ICommandHandler<DeleteFormCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFormCommand(formId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithTags(Tags.Forms);
    }
}

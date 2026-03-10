using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Form;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meta/pages/{pageId}/forms", async (
            string pageId,
            CreateFormRequest request,
            ICommandHandler<CreateFormCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateFormCommand(
                pageId,
                request.Name,
                request.Locale,
                request.PrivacyPolicy.Url,
                request.PrivacyPolicy.LinkText,
                request.Questions,
                request.FollowUpActionUrl);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(id => Results.Created($"meta/forms/{id}", id), CustomResults.Problem);
        })
        .WithTags(Tags.Forms)
        .Produces<FormResponse>(200);
    }
}

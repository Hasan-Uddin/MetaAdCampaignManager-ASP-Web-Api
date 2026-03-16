using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.CreateFromFormTemplate;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.Forms;

internal sealed class CreateFromFormTemplate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meta/pages/{pageId}/forms/form-template", async (
            string pageId,
            CreateFromFormTemplateRequest request,
            ICommandHandler<CreateFormFromTemplateCommand, string> handler,
            CancellationToken ct) =>
        {
            var command = new CreateFormFromTemplateCommand(
                request.TemplateId,
                pageId,
                request.Name,
                request.PrivacyPolicyUrl,
                request.PrivacyPolicyLinkText,
                request.FollowUpActionUrl);

            Result<string> result = await handler.Handle(command, ct);
            return result.Match(id => Results.Created($"meta/forms/{id}", id), CustomResults.Problem);
        })
        .WithTags(Tags.Forms)
        .Produces<FormResponse>(200);
    }
}

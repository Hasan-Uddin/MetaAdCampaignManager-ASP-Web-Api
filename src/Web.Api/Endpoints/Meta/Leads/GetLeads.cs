using Application.Abstractions.Messaging;
using Application.Features.Meta.Leads.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.Leads;

internal sealed class GetLeads : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/forms/{formId}/leads", async (
            string formId,
            IQueryHandler<GetLeadsQuery, List<LeadResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<LeadResponse>> result = await handler.Handle(new GetLeadsQuery(formId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Leads)
        .Produces<List<LeadResponse>>();
    }
}

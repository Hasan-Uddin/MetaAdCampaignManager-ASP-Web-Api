using Application.Abstractions.Messaging;
using Application.Features.Meta.Leads.GetStructuredLeads;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.Leads;

internal sealed class GetStructuredLead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/forms/{formId}/structured-leads", async (
            string formId,
            IQueryHandler<GetStructuredLeadsQuery, List<StructuredLeadResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<StructuredLeadResponse>> result = await handler.Handle(new GetStructuredLeadsQuery(formId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Leads);
    }
}

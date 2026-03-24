using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Leads.GetStructuredLeads;

public sealed record GetStructuredLeadsQuery(string FormId) : IQuery<List<StructuredLeadResponse>>;

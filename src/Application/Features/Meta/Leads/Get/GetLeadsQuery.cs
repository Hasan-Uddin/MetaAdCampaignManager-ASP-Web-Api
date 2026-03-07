using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Leads.Get;

public sealed record GetLeadsQuery(string FormId) : IQuery<List<LeadResponse>>;

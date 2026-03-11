using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Forms.Get;

public sealed record GetFormsQuery(string PageId) : IQuery<List<FormResponse>>;

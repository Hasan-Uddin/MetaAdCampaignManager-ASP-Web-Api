using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;

namespace Application.Features.Meta.Forms.Get;

public sealed record GetFormsQuery(string PageId) : IQuery<List<FormResponse>>;

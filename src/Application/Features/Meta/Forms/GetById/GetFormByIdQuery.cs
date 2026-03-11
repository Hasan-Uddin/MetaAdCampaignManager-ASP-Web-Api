using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Forms.GetById;

public sealed record GetFormByIdQuery(string FormId) : IQuery<FormResponse>;

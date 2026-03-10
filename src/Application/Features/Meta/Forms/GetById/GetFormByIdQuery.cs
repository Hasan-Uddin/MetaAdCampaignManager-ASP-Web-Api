using Application.Abstractions.Messaging;
using Application.Features.Meta.Forms;

namespace Application.Features.Meta.Forms.GetById;

public sealed record GetFormByIdQuery(string FormId) : IQuery<FormResponse>;

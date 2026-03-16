using Application.Abstractions.Messaging;

namespace Application.Features.Meta.FormTemplates.GetById;

public sealed record GetFormTemplateByIdQuery(Guid TemplateId) : IQuery<FormTemplateResponse>;

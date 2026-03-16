using Application.Abstractions.Messaging;

namespace Application.Features.Meta.FormTemplates.Get;

public sealed record GetFormTemplatesQuery : IQuery<List<FormTemplateResponse>>;

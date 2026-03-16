using Application.Abstractions.Messaging;

namespace Application.Features.Meta.FormTemplates.Delete;

public sealed record DeleteFormTemplateCommand(Guid TemplateId) : ICommand;

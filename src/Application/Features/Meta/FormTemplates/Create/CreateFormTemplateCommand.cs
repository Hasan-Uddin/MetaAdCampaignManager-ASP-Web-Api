using Application.Abstractions.Messaging;

namespace Application.Features.Meta.FormTemplates.Create;

public sealed record CreateFormTemplateCommand(
    string Name,
    string Description,
    List<QuestionRequest> Questions,
    bool IsDefault) : ICommand<Guid>;

using Application.Abstractions.Messaging;

namespace Application.Features.Meta.FormTemplates.Update;


public sealed record UpdateFormTemplateCommand(
    Guid TemplateId,
    string Name,
    string Description,
    List<QuestionRequest> Questions,
    bool IsDefault) : ICommand;

using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Forms.Delete;

public sealed record DeleteFormCommand(string FormId) : ICommand;

using Application.Abstractions.Messaging;

namespace Application.Features.Meta.MetaSettings.Update;

public sealed record UpdateMetaAppCredentialsCommand(
    string AppId,
    string AppSecret) : ICommand;

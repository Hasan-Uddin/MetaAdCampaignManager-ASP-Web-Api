using Application.Abstractions.Messaging;

namespace Application.Features.Meta.MetaSettings.Create;

public sealed record CreateeMetaSettingsCommand(
    string AppId,
    string AppSecret,
    string UserToken,
    string WebhookVerifyToken) : ICommand;

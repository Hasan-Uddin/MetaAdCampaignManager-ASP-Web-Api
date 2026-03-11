using Application.Abstractions.Messaging;

namespace Application.Features.Auth.MetaOAuth;

public sealed record MetaOAuthCallbackCommand(
    string Code,
    string RedirectUri) : ICommand<MetaOAuthCallbackCommandResponse>;

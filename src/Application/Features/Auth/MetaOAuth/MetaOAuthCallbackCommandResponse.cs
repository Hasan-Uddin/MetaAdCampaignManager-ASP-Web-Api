using Application.Abstractions.Authentication;

namespace Application.Features.Auth.MetaOAuth;


public sealed record MetaOAuthCallbackCommandResponse(TokenResponse TokenResult, Guid UserId);

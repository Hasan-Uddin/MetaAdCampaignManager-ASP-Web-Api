
using Application.Abstractions.Authentication;

namespace Application.Features.Auth.Login;

public sealed record GoogleLoginCommandResponse(TokenResult tokenResult, Guid UserId);

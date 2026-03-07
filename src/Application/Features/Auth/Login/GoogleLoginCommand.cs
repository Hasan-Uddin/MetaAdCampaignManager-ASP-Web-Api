using Application.Abstractions.Messaging;

namespace Application.Features.Auth.Login;

public record GoogleLoginCommand(string Code) : ICommand<GoogleLoginCommandResponse>;

using Application.Abstractions.Messaging;

namespace Application.Features.Users.GetMe;

public sealed record GetMeQuery() : IQuery<UserResponse>;

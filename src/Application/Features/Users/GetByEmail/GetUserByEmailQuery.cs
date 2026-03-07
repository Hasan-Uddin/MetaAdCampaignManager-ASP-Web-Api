using Application.Abstractions.Messaging;

namespace Application.Features.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;

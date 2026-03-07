using Application.Abstractions.Messaging;

namespace Application.Features.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Users.GetMe;

internal sealed class GetMeQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetMeQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        Guid? userId = userContext.UserId;
        if (userId is null)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

        UserResponse? user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PictureUrl = u.PictureUrl
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound());
        }

        if (user.Id != userContext.UserId)
        {
            return Result.Failure<UserResponse>(UserErrors.Unauthorized());
        }

        return user;
    }
}

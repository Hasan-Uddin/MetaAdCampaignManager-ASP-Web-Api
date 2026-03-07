using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Messaging;
using Domain.Users;
using SharedKernel;

namespace Application.Features.Auth.Login;

public sealed class GoogleLoginCommandHandler(
    IGoogleAuthService googleService,
    IUserRepository userRepository,
    ITokenProvider tokenProvider) : ICommandHandler<GoogleLoginCommand, GoogleLoginCommandResponse>
{

    async Task<Result<GoogleLoginCommandResponse>> ICommandHandler<GoogleLoginCommand, GoogleLoginCommandResponse>.Handle(GoogleLoginCommand command, CancellationToken cancellationToken)
    {
        GoogleUserInfo googleUser = await googleService.ExchangeCodeAsync(command.Code);

        User? user = await userRepository.GetByEmailAsync(googleUser.Email);

        if (user == null)
        {
            user = User.Create(googleUser.Email, googleUser.Name);
            user.GoogleId = googleUser.GoogleId;
            user.PictureUrl = googleUser.PictureUrl;
            user.GoogleRefreshToken = googleUser.GoogleRefreshToken;
            await userRepository.AddAsync(user);
        }
        else
        {
            user.Name = googleUser.Name;
        }

        await userRepository.SaveChangesAsync();

        return Result.Success(new GoogleLoginCommandResponse(tokenProvider.Create(user), user.Id));
    }
}

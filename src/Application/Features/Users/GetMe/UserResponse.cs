namespace Application.Features.Users.GetMe;

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string Name { get; init; }

    public string PictureUrl { get; init; }
}

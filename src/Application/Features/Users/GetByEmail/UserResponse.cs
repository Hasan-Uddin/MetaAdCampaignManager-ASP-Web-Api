namespace Application.Features.Users.GetByEmail;

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string Name { get; init; }

    public string PictureUrl { get; init; }
}

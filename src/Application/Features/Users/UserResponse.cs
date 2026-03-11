namespace Application.Features.Users;

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string? Email { get; init; }

    public string? FacebookId { get; set; }

    public string Name { get; init; }

    public string? PictureUrl { get; init; }
    
    public DateTime CreatedAt { get; set; }
}

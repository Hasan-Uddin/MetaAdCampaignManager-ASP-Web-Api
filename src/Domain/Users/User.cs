using Domain.MetaSettings;
using SharedKernel;

namespace Domain.Users;

public sealed class User : Entity
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; }
    public string? FacebookId { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public MetaSetting? MetaSetting { get; set; }

    public static User Create(string? email, string name)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User CreateFromFacebook(
        string facebookId,
        string name,
        string? email,
        string? pictureUrl)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FacebookId = facebookId,
            Name = name,
            Email = email,
            PictureUrl = pictureUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}

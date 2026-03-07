
namespace Application.Abstractions.Authentication;

public interface IGoogleAuthSettings
{
    string ClientId { get; }
    string RedirectUri { get; }
}

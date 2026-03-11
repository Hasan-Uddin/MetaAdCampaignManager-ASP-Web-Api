namespace Infrastructure.Services.Meta;

public sealed class MetaApiOptions
{
    public const string SectionName = "MetaApi";
    public string AppID { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string AdAccountId { get; init; } = string.Empty; //only id, without "act_" prefix
    public string AppSecret { get; init; } = string.Empty;
    public string WebhookVerifyToken { get; init; } = string.Empty;
    public string PageAccessToken { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://graph.facebook.com/v25.0";
}

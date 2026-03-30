
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.WhatsApp;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services.WhatsApp;

internal sealed class WhatsAppService(
    HttpClient httpClient,
    ILogger<WhatsAppService> logger) : IWhatsAppService
{
    private const string BaseUrl = $"https://graph.facebook.com/v25.0/";

    public async Task<Result<string>> SendMessageAsync(
        string phoneNumberId, string toPhone, string body, string accessToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}{phoneNumberId}/messages?access_token={accessToken}";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = toPhone,
                type = "text",
                text = new { body }
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, payload, ct);
            response.EnsureSuccessStatusCode();

            SendMessageResponse? result = await response.Content
                .ReadFromJsonAsync<SendMessageResponse>(cancellationToken: ct);

            return result?.Messages?.FirstOrDefault()?.Id
                ?? Result.Failure<string>(Error.Failure("WhatsApp.Send", "No message ID returned."));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send WhatsApp message to {Phone}", toPhone);
            return Result.Failure<string>(Error.Failure("WhatsApp.Unavailable", ex.Message));
        }
    }

    public async Task<Result<WhatsAppBusinessInfo>> GetFirstBusinessAccountAsync(
    string accessToken, CancellationToken ct = default)
    {
        try
        {
            // get businesses the user owns
            string url = $"{BaseUrl}me/businesses?fields=id,name&access_token={accessToken}";
            MetaPaginatedResponse<BusinessResponse>? response =
                await httpClient.GetFromJsonAsync<MetaPaginatedResponse<BusinessResponse>>(url, ct);

            BusinessResponse? business = response?.Data.FirstOrDefault();
            if (business is null)
            {
                return Result.Failure<WhatsAppBusinessInfo>(
                    Error.Failure("WhatsApp.NoBusiness", "No business account found."));
            }

            // get WABA owned by that business
            string wabaUrl = $"{BaseUrl}{business.Id}/owned_whatsapp_business_accounts?fields=id,name&access_token={accessToken}";
            MetaPaginatedResponse<WabaResponse>? wabaResponse =
                await httpClient.GetFromJsonAsync<MetaPaginatedResponse<WabaResponse>>(wabaUrl, ct);

            WabaResponse? waba = wabaResponse?.Data.FirstOrDefault();
            if (waba is null)
            {
                return Result.Failure<WhatsAppBusinessInfo>(
                    Error.Failure("WhatsApp.NoWABA", "No WhatsApp Business Account found under this business."));
            }

            return new WhatsAppBusinessInfo(waba.Id);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch WhatsApp business account");
            return Result.Failure<WhatsAppBusinessInfo>(Error.Failure("WhatsApp.Unavailable", ex.Message));
        }
    }

    public async Task<Result<WhatsAppPhoneNumberInfo>> GetFirstPhoneNumberAsync(
        string wabaId, string accessToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}{wabaId}/phone_numbers?fields=id,display_phone_number&access_token={accessToken}";
            MetaPaginatedResponse<PhoneNumberResponse>? response =
                await httpClient.GetFromJsonAsync<MetaPaginatedResponse<PhoneNumberResponse>>(url, ct);

            PhoneNumberResponse? phone = response?.Data.FirstOrDefault();
            if (phone is null)
            {
                return Result.Failure<WhatsAppPhoneNumberInfo>(
                    Error.Failure("WhatsApp.NoPhone", "No phone number found."));
            }

            return new WhatsAppPhoneNumberInfo(phone.Id, phone.DisplayPhoneNumber);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch WhatsApp phone numbers");
            return Result.Failure<WhatsAppPhoneNumberInfo>(Error.Failure("WhatsApp.Unavailable", ex.Message));
        }
    }

    // ===============  Private response models =================

    private sealed record MetaPaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data);

    private sealed record BusinessResponse(
        [property: JsonPropertyName("id")] string Id);

    private sealed record WabaResponse(
        [property: JsonPropertyName("id")] string Id);

    private sealed record PhoneNumberResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("display_phone_number")] string DisplayPhoneNumber);

    private sealed record SendMessageResponse(
        [property: JsonPropertyName("messages")] List<SentMessage>? Messages);

    private sealed record SentMessage(
        [property: JsonPropertyName("id")] string Id);
}

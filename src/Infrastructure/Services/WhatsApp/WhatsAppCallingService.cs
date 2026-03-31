using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.WhatsApp;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services.WhatsApp;

internal sealed class WhatsAppCallingService(
    HttpClient httpClient,
    ILogger<WhatsAppCallingService> logger) : IWhatsAppCallingService
{
    private const string BaseUrl = $"https://graph.facebook.com/v25.0/";

    public async Task<Result<WhatsAppCallConfigSnapshot>> GetCallSettingsAsync(
        string phoneNumberId, string accessToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}{phoneNumberId}/settings?access_token={accessToken}";
            CallSettingsResponse? response = await httpClient.GetFromJsonAsync<CallSettingsResponse>(url, ct);

            if (response?.Calling is null)
            {
                return Result.Failure<WhatsAppCallConfigSnapshot>(
                    Error.Failure("WhatsApp.CallSettings", "No calling settings returned."));
            }

            return new WhatsAppCallConfigSnapshot(
                response.Calling.Status == "ENABLED",
                response.Calling.InboundCallControl == "ENABLED",
                response.Calling.CallbackRequests == "ENABLED",
                response.Calling.CallHours?.Mode ?? "ALWAYS");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get WhatsApp call settings for {PhoneNumberId}", phoneNumberId);
            return Result.Failure<WhatsAppCallConfigSnapshot>(Error.Failure("WhatsApp.Unavailable", ex.Message));
        }
    }

    public async Task<Result> UpdateCallSettingsAsync(
        string phoneNumberId, string accessToken, UpdateCallSettingsRequest request, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}{phoneNumberId}/settings?access_token={accessToken}";

            var payload = new
            {
                calling = new
                {
                    status = request.CallingEnabled ? "ENABLED" : "DISABLED",
                    inbound_call_control = request.InboundCallsEnabled ? "ENABLED" : "DISABLED",
                    callback_requests = request.CallbackRequestsEnabled ? "ENABLED" : "DISABLED",
                    call_hours = new
                    {
                        mode = request.CallHoursMode.ToUpper(CultureInfo.InvariantCulture),
                        business_hours = request.BusinessHours.Select(h => new
                        {
                            day = h.Day.ToUpper(CultureInfo.InvariantCulture),
                            open_time = h.OpenTime,
                            close_time = h.CloseTime
                        })
                    }
                }
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, payload, ct);
            response.EnsureSuccessStatusCode();
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update WhatsApp call settings for {PhoneNumberId}", phoneNumberId);
            return Result.Failure(Error.Failure("WhatsApp.Unavailable", ex.Message));
        }
    }

    // =============== Private response models =========================

    private sealed record CallSettingsResponse(
        [property: JsonPropertyName("calling")] CallingSettings? Calling);

    private sealed record CallingSettings(
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("inbound_call_control")] string? InboundCallControl,
        [property: JsonPropertyName("callback_requests")] string? CallbackRequests,
        [property: JsonPropertyName("call_hours")] CallHoursSettings? CallHours);

    private sealed record CallHoursSettings(
        [property: JsonPropertyName("mode")] string Mode,
        [property: JsonPropertyName("business_hours")] List<BusinessHourEntry>? BusinessHours);

    private sealed record BusinessHourEntry(
        [property: JsonPropertyName("day")] string Day,
        [property: JsonPropertyName("open_time")] string OpenTime,
        [property: JsonPropertyName("close_time")] string CloseTime);
}

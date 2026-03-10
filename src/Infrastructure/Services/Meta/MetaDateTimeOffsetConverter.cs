using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta;

public class MetaDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        // Convert timezone from +0600 -> +06:00
        if (value.Length >= 5 &&
            (value[^5] == '+' || value[^5] == '-') &&
            value[^3] != ':')
        {
            value = value.Insert(value.Length - 2, ":");
        }

        return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            value.ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture)
        );
    }
}

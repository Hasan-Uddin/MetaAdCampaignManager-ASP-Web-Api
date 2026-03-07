using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta;

public class MetaDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value == null)
        {
            return default;
        }

        // Insert colon before last 2 digits of timezone if missing
        if (value.Length > 5 && value[^5] != ':' && (value[^5] == '+' || value[^5] == '-'))
        {
            value = value[..^2] + ":" + value[^2..];
        }

        return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture));
}

using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;


    public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public DateTimeJsonConverter(string format)
    {
        _format = format;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), _format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}


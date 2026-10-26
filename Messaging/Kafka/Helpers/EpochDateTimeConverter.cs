using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a <see cref="DateTime"/> to and from its Unix epoch time representation
/// in milliseconds for JSON serialization and deserialization.
/// </summary>
/// <remarks>
/// This converter is used to handle <see cref="DateTime"/> objects by serializing
/// them into Unix epoch time in milliseconds, and deserializing Unix epoch time
/// back into <see cref="DateTime"/> instances. It ensures compatibility with
/// APIs or systems that represent date and time as the number of milliseconds
/// elapsed since the Unix epoch (January 1, 1970 UTC).
/// During serialization, if the provided <see cref="DateTime"/> is not in UTC,
/// it is converted to UTC before representing it in milliseconds.
/// During deserialization, the Unix epoch time in milliseconds is converted
/// into a UTC <see cref="DateTime"/> object.
/// </remarks>
public class EpochDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Reads a JSON value representing a Unix epoch time in milliseconds and converts
    /// it to a <see cref="DateTime"/> object in UTC.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert. This is ignored in this implementation.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    /// <returns>A <see cref="DateTime"/> object in UTC corresponding to the Unix epoch time in milliseconds.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var epochMillis = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(epochMillis).UtcDateTime;
    }

    /// <summary>
    /// Writes a <see cref="DateTime"/> object to its Unix epoch time representation in milliseconds
    /// for JSON serialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The <see cref="DateTime"/> object to be converted and written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var dto = value.Kind == DateTimeKind.Utc
            ? new DateTimeOffset(value)
            : new DateTimeOffset(value.ToUniversalTime());

        writer.WriteNumberValue(dto.ToUnixTimeMilliseconds());
    }
}
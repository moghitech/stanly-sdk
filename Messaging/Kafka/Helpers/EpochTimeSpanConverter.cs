using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a <see cref="TimeSpan"/> to and from its total milliseconds representation
/// for JSON serialization and deserialization.
/// </summary>
/// <remarks>
/// This converter serializes <see cref="TimeSpan"/> objects as the total number of
/// milliseconds in the time span, and deserializes milliseconds back into
/// <see cref="TimeSpan"/> instances.
/// </remarks>
public class EpochTimeSpanConverter : JsonConverter<TimeSpan>
{
    /// <summary>
    /// Reads a JSON value representing total milliseconds and converts
    /// it to a <see cref="TimeSpan"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert. This is ignored in this implementation.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    /// <returns>A <see cref="TimeSpan"/> object corresponding to the total milliseconds.</returns>
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var milliseconds = reader.GetInt64();
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    /// <summary>
    /// Writes a <see cref="TimeSpan"/> object to its total milliseconds representation
    /// for JSON serialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The <see cref="TimeSpan"/> object to be converted and written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        var milliseconds = (long)value.TotalMilliseconds;
        writer.WriteNumberValue(milliseconds);
    }
}
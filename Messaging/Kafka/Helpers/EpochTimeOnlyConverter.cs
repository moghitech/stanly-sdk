using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a <see cref="TimeOnly"/> to and from its total milliseconds representation
/// since midnight for JSON serialization and deserialization.
/// </summary>
/// <remarks>
/// This converter serializes <see cref="TimeOnly"/> objects as the number of milliseconds
/// elapsed since midnight (00:00:00), and deserializes milliseconds back into
/// <see cref="TimeOnly"/> instances.
/// </remarks>
public class EpochTimeOnlyConverter : JsonConverter<TimeOnly>
{
    /// <summary>
    /// Reads a JSON value representing milliseconds since midnight and converts
    /// it to a <see cref="TimeOnly"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert. This is ignored in this implementation.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    /// <returns>A <see cref="TimeOnly"/> object corresponding to the milliseconds since midnight.</returns>
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var milliseconds = reader.GetInt64();
        return TimeOnly.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <summary>
    /// Writes a <see cref="TimeOnly"/> object to its milliseconds since midnight representation
    /// for JSON serialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The <see cref="TimeOnly"/> object to be converted and written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        var milliseconds = (long)value.ToTimeSpan().TotalMilliseconds;
        writer.WriteNumberValue(milliseconds);
    }
}
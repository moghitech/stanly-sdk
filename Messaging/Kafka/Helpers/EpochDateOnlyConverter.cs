using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a <see cref="DateOnly"/> to and from its Unix epoch time representation
/// in days for JSON serialization and deserialization.
/// </summary>
/// <remarks>
/// This converter serializes <see cref="DateOnly"/> objects as the number of days
/// elapsed since the Unix epoch (January 1, 1970), and deserializes days back into
/// <see cref="DateOnly"/> instances.
/// </remarks>
public class EpochDateOnlyConverter : JsonConverter<DateOnly>
{
    private static readonly DateOnly UnixEpoch = new DateOnly(1970, 1, 1);

    /// <summary>
    /// Reads a JSON value representing the number of days since Unix epoch and converts
    /// it to a <see cref="DateOnly"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert. This is ignored in this implementation.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    /// <returns>A <see cref="DateOnly"/> object corresponding to the days since Unix epoch.</returns>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var days = reader.GetInt32();
        return UnixEpoch.AddDays(days);
    }

    /// <summary>
    /// Writes a <see cref="DateOnly"/> object to its days since Unix epoch representation
    /// for JSON serialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The <see cref="DateOnly"/> object to be converted and written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>. This is ignored in this implementation.</param>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var days = value.DayNumber - UnixEpoch.DayNumber;
        writer.WriteNumberValue(days);
    }
}
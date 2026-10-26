using System.Text.Json;
using System.Text.Json.Serialization;
using Fusion.Messaging.Kafka.Events;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// JSON converter for Debezium CDC operation field mapping single characters to CdcOperation enum.
/// </summary>
public class CdcOperationConverter : JsonConverter<CdcOperation>
{
    /// <summary>
    /// Reads and converts a JSON string representing a Debezium Change Data Capture (CDC) operation
    /// into a corresponding <see cref="CdcOperation"/> enum value.
    /// </summary>
    /// <param name="reader">A reference to the <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type of object to convert, which in this case is <see cref="CdcOperation"/>.</param>
    /// <param name="options">Options to control the behavior during conversion.</param>
    /// <returns>The <see cref="CdcOperation"/> enum value that matches the parsed JSON string.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value does not represent a valid Debezium CDC operation.</exception>
    public override CdcOperation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return value switch
        {
            "c" => CdcOperation.Create,
            "u" => CdcOperation.Update,
            "d" => CdcOperation.Delete,
            "r" => CdcOperation.Read,
            "t" => CdcOperation.Truncate,
            _ => CdcOperation.Unknown
        };
    }

    /// <summary>
    /// Writes a <see cref="CdcOperation"/> enum value as a single-character string
    /// representing the corresponding Debezium Change Data Capture (CDC) operation.
    /// </summary>
    /// <param name="writer">A reference to the <see cref="Utf8JsonWriter"/> to write the data to.</param>
    /// <param name="value">The <see cref="CdcOperation"/> enum value to convert and write.</param>
    /// <param name="options">Options to control the behavior during serialization.</param>
    /// <exception cref="JsonException">Thrown when the provided <see cref="CdcOperation"/> value is not valid or recognized.</exception>
    public override void Write(Utf8JsonWriter writer, CdcOperation value, JsonSerializerOptions options)
    {
        var operation = value switch
        {
            CdcOperation.Create => "c",
            CdcOperation.Update => "u",
            CdcOperation.Delete => "d",
            CdcOperation.Read => "r",
            CdcOperation.Truncate => "t",
            _ => nameof(CdcOperation.Unknown)
        };

        writer.WriteStringValue(operation);
    }
}
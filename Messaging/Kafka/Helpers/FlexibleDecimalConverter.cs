using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a <see cref="decimal"/> to and from JSON, handling both numeric and string representations.
/// </summary>
/// <remarks>
/// This converter can deserialize decimal values from both JSON numbers and JSON strings,
/// making it compatible with APIs that may send numeric values as strings.
/// During serialization, the value is always written as a JSON number.
/// </remarks>
public class FlexibleDecimalConverter : JsonConverter<decimal>
{
    /// <summary>
    /// Reads a JSON value (either a number or string) and converts it to a <see cref="decimal"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>.</param>
    /// <returns>A <see cref="decimal"/> value parsed from the JSON.</returns>
    /// <exception cref="JsonException">Thrown when the value cannot be converted to a decimal.</exception>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to convert \"{stringValue}\" to decimal.");
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    /// <summary>
    /// Writes a <see cref="decimal"/> value as a JSON number.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The <see cref="decimal"/> value to be written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>.</param>
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
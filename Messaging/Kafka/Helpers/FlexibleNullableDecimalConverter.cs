using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Converts a nullable <see cref="decimal"/> to and from JSON, handling both numeric and string representations.
/// </summary>
/// <remarks>
/// This converter extends <see cref="FlexibleDecimalConverter"/> to handle nullable decimals,
/// properly handling JSON null values.
/// </remarks>
public class FlexibleNullableDecimalConverter : JsonConverter<decimal?>
{
    /// <summary>
    /// Reads a JSON value (number, string, or null) and converts it to a nullable <see cref="decimal"/>.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type of the object to convert.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>.</param>
    /// <returns>A nullable <see cref="decimal"/> value parsed from the JSON, or null.</returns>
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }
            
            if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            throw new JsonException($"Unable to convert \"{stringValue}\" to decimal.");
        }

        throw new JsonException($"Unexpected token type: {reader.TokenType}");
    }

    /// <summary>
    /// Writes a nullable <see cref="decimal"/> value as either a JSON number or null.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> used to write the JSON data.</param>
    /// <param name="value">The nullable <see cref="decimal"/> value to be written.</param>
    /// <param name="options">Optional settings for the <see cref="JsonSerializer"/>.</param>
    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
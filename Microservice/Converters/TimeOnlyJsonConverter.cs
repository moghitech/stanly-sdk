using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Microservice.Converters;

internal class TimeOnlyJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(TimeOnly) ||
               (typeToConvert.IsGenericType && IsNullableTimeOnly(typeToConvert));
    }

    private static bool IsNullableTimeOnly(Type typeToConvert)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);

        return underlyingType is not null && underlyingType == typeof(TimeOnly);
    }
    
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return typeToConvert.IsGenericType
            ? new JsonNullableTimeOnlyConverter()
            : new JsonStandardTimeOnlyConverter();
    }
}

internal class JsonStandardTimeOnlyConverter : JsonConverter<TimeOnly>
{
    private const string TimeOnlyStringFormat = "HH:mm:ss";
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"The JSON value could not be converted to {typeof(TimeOnly)}.");

        var value = reader.GetString()!;
        try
        {
            return TimeOnly.ParseExact(value, TimeOnlyStringFormat, CultureInfo.InvariantCulture);
        }
        catch (Exception parseException)
        {
            throw new JsonException($"The JSON value '{value}' could not be converted to {typeof(TimeOnly)}.",
                parseException);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(TimeOnlyStringFormat, CultureInfo.InvariantCulture));
}

internal class JsonNullableTimeOnlyConverter : JsonConverter<TimeOnly?>
{
    private const string TimeOnlyStringFormat = "HH:mm:ss";

    public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"The JSON value could not be converted to {typeof(TimeOnly?)}.");

        var value = reader.GetString()!;
        try
        {
            return TimeOnly.ParseExact(value, TimeOnlyStringFormat, CultureInfo.InvariantCulture);
        }
        catch (Exception parseException)
        {
            throw new JsonException($"The JSON value '{value}' could not be converted to {typeof(TimeSpan?)}.",
                parseException);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        => writer.WriteStringValue(value!.Value.ToString(TimeOnlyStringFormat, CultureInfo.InvariantCulture));
}
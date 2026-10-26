using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fusion.Microservice.Converters;

internal class TimeSpanJsonConverter : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert == typeof(TimeSpan) ||
		       (typeToConvert.IsGenericType && IsNullableTimeSpan(typeToConvert));
	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		return typeToConvert.IsGenericType
			? new JsonNullableTimeSpanConverter()
			: new JsonStandardTimeSpanConverter();
	}

	private static bool IsNullableTimeSpan(Type typeToConvert)
	{
		var underlyingType = Nullable.GetUnderlyingType(typeToConvert);

		return underlyingType != null && underlyingType == typeof(TimeSpan);
	}

	private class JsonStandardTimeSpanConverter : JsonConverter<TimeSpan>
	{
		public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
				throw new JsonException($"The JSON value could not be converted to {typeof(TimeSpan)}.");

			var value = reader.GetString()!;
			try
			{
				return TimeSpan.ParseExact(value, "c", CultureInfo.InvariantCulture);
			}
			catch (Exception parseException)
			{
				throw new JsonException($"The JSON value '{value}' could not be converted to {typeof(TimeSpan)}.",
					parseException);
			}
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
	}

	private class JsonNullableTimeSpanConverter : JsonConverter<TimeSpan?>
	{
		public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
				throw new JsonException($"The JSON value could not be converted to {typeof(TimeSpan?)}.");

			var value = reader.GetString()!;
			try
			{
				return TimeSpan.ParseExact(value, "c", CultureInfo.InvariantCulture);
			}
			catch (Exception parseException)
			{
				throw new JsonException($"The JSON value '{value}' could not be converted to {typeof(TimeSpan?)}.",
					parseException);
			}
		}

		public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value!.Value.ToString("c", CultureInfo.InvariantCulture));
	}
}
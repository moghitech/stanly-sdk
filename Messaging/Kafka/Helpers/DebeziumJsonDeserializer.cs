using System.Text.Json;
using Confluent.Kafka;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// Provides a custom deserializer implementation for handling JSON data produced by Debezium.
/// This deserializer leverages the System.Text.Json library to convert JSON-encoded data into
/// strongly-typed objects of type <typeparamref name="T"/>. It enables seamless integration with
/// Kafka consumer applications that process Debezium change events.
/// </summary>
/// <typeparam name="T">
/// The target type into which the JSON data will be deserialized. This type must be compatible
/// with System.Text.Json serialization and deserialization mechanisms.
/// </typeparam>
/// <remarks>
/// The deserialization process uses a <see cref="JsonSerializerOptions"/> instance for configuring
/// the behavior of the underlying serializer. Properties such as case sensitivity and custom
/// converters can be customized via these options. If the input byte data is null, the deserializer
/// returns the default value of <typeparamref name="T"/>.
/// </remarks>
public class DebeziumJsonDeserializer<T>(JsonSerializerOptions options) : IDeserializer<T>
{
    /// <summary>
    /// Deserializes a JSON-encoded byte array into an object of type <typeparamref name="T"/>.
    /// Uses the <see cref="System.Text.Json.JsonSerializer"/> for conversion between JSON and objects.
    /// </summary>
    /// <param name="data">
    /// The JSON-encoded byte array input to be deserialized.
    /// </param>
    /// <param name="isNull">
    /// A flag indicating whether the input data is null. If true, the method will return the default
    /// value of type <typeparamref name="T"/>.
    /// </param>
    /// <param name="context">
    /// The serialization context provided by Kafka. This contains additional metadata about the serialization process.
    /// </param>
    /// <returns>
    /// An object of type <typeparamref name="T"/> deserialized from the input JSON data. If <paramref name="isNull"/> is true,
    /// the default value for <typeparamref name="T"/> is returned.
    /// </returns>
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return (isNull ? default : JsonSerializer.Deserialize<T>(data, options))!;
    }
}
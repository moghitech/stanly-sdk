using System.Net.Mime;
using System.Text.Json;
using Confluent.Kafka;
using Fusion.Messaging.Kafka.Helpers;
using MassTransit.KafkaIntegration.Serializers;

namespace Fusion.Messaging.Kafka.Debezium.Masstransit;

/// <summary>
/// Factory class for creating Kafka serializers and deserializers tailored for use with Debezium JSON events.
/// This implementation ensures that Kafka message payloads follow Debezium's JSON serialization format,
/// enabling seamless integration with Debezium for change data capture (CDC) use cases.
/// </summary>
public class DebeziumKafkaSerializerFactory : IKafkaSerializerFactory
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new EpochDateTimeConverter(),
            new EpochDateOnlyConverter(),
            new EpochTimeOnlyConverter(),
            new EpochTimeSpanConverter(),
            new FlexibleDecimalConverter(),
            new FlexibleNullableDecimalConverter(),
        }
    };

    /// <summary>
    /// Creates an instance of the Debezium JSON deserializer for the specified type, using pre-configured
    /// JSON serialization settings. The deserializer processes messages in a format compatible with Debezium.
    /// </summary>
    /// <typeparam name="T">The type to which the deserialized data will be mapped.</typeparam>
    /// <returns>An instance of <see cref="IDeserializer{T}"/> designed for handling Debezium JSON-formatted messages.</returns>
    public IDeserializer<T> GetDeserializer<T>()
    {
        return new DebeziumJsonDeserializer<T>(_serializerOptions);
    }

    /// <summary>
    /// Creates an instance of the Debezium JSON serializer for the specified type, utilizing pre-configured
    /// JSON serialization settings. The serializer ensures that objects are serialized into UTF-8 encoded
    /// JSON byte arrays compatible with Debezium's format.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized.</typeparam>
    /// <returns>An instance of <see cref="IAsyncSerializer{T}"/> for handling Debezium-compatible serialization.</returns>
    public IAsyncSerializer<T> GetSerializer<T>()
    {
        return new DebeziumJsonSerializer<T>(_serializerOptions);
    }

    /// <summary>
    /// Gets the content type used for Kafka message serialization and deserialization.
    /// This property indicates the MIME type of the serialized data, which in this case
    /// is "application/json", ensuring compatibility with JSON-based messaging formats.
    /// </summary>
    public ContentType ContentType { get; } = new("application/json");
}
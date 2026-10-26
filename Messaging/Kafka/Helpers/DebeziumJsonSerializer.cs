using System.Text.Json;
using Confluent.Kafka;

namespace Fusion.Messaging.Kafka.Helpers;

/// <summary>
/// A custom JSON serializer for use with Kafka that utilizes the System.Text.Json library.
/// This serializer is designed to handle serialization of objects into UTF-8 encoded JSON byte arrays
/// suitable for Debezium and Kafka integration.
/// </summary>
/// <typeparam name="T">The type of the object to be serialized.</typeparam>
public class DebeziumJsonSerializer<T>(JsonSerializerOptions options) : IAsyncSerializer<T>
{
    /// <summary>
    /// Asynchronously serializes an object of type <typeparamref name="T"/> into a UTF-8 encoded JSON byte array.
    /// </summary>
    /// <param name="data">The object to be serialized. If null, the result will also be null.</param>
    /// <param name="context">The serialization context provided by Kafka, which contains metadata about the serialization process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the serialized JSON byte array.</returns>
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        if (data == null)
            return Task.FromResult<byte[]>(null!);

        var json = JsonSerializer.SerializeToUtf8Bytes(data, options);
        return Task.FromResult(json);
    }
}
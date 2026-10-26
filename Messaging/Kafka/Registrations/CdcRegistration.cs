using Confluent.Kafka;
using Fusion.Messaging.Kafka.Abstractions;
using Fusion.Messaging.Kafka.Debezium.Masstransit;
using Fusion.Messaging.Kafka.Handlers;
using Fusion.Microservice.Extensions;
using Fusion.Microservice.Extensions.Options;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Messaging.Kafka.Registrations;

/// <summary>
/// Provides extension methods for registering Kafka messaging functionality
/// within an application using the MassTransit framework and Kafka transport.
/// </summary>
public static class CdcRegistration
{
    /// <summary>
    /// Configures Kafka message consumption by setting up required consumers, topics, and Kafka transport
    /// using MassTransit and Kafka configurations.
    /// </summary>
    /// <param name="builder">The web application builder to which Kafka-related dependencies will be added.</param>
    /// <param name="configure">
    /// An action to configure the Kafka consumer by adding topic-specific consumers and providing their configuration.
    /// </param>
    public static void AddCdc(this WebApplicationBuilder builder,
        Action<IKafkaHandlerConfigurator> configure)
    {
        var configurator = new KafkaHandlerConfigurator();
        configure(configurator);

        var serviceOptions = builder.Configuration.GetServiceOptions();
        var kafkaOption = serviceOptions.KafkaOptions;

        if (kafkaOption.IsEnabled == false)
            return;

        foreach (var registration in configurator.Registrations)
        {
            if (registration.HandlerType != null)
            {
                var handlerInterfaceType = typeof(ICdcEventHandler<>).MakeGenericType(registration.EventType);
                builder.Services.AddScoped(handlerInterfaceType, registration.HandlerType);
            }
        }

        builder.Services.AddMassTransit<IKafkaBus>(busConfigurator =>
        {
            busConfigurator.UsingInMemory();

            busConfigurator.AddRider(rider =>
            {
                foreach (var registration in configurator.Registrations)
                {
                    rider.AddConsumer(registration.ConsumerType);
                }

                rider.UsingKafka((context, k) =>
                {
                    k.SetSerializationFactory(new DebeziumKafkaSerializerFactory());

                    k.Host(kafkaOption.BootstrapServer, hostConfiguration =>
                    {
                        hostConfiguration.UseSasl(sasl =>
                        {
                            sasl.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                            sasl.Mechanism = SaslMechanism.ScramSha512;
                            sasl.Username = kafkaOption.Username;
                            sasl.Password = kafkaOption.Password;
                        });
                    });

                    foreach (var registration in configurator.Registrations)
                    {
                        var method = typeof(CdcRegistration).GetMethod(nameof(ConfigureEndpoint),
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                            .MakeGenericMethod(registration.ConsumerType, registration.EventType);

                        method.Invoke(null,
                        [
                            k, registration.TopicWildcard, kafkaOption.GroupId, context, kafkaOption
                        ]);
                    }
                });
            });
        });
    }

    private static void ConfigureEndpoint<TConsumer, TEvent>(
        IKafkaFactoryConfigurator configurator,
        string topic,
        string groupId,
        IRiderRegistrationContext context,
        KafkaOptions options)
        where TConsumer : class, IConsumer<TEvent>
        where TEvent : class
    {
        configurator.TopicEndpoint<TEvent>(topic, groupId, e =>
        {
            e.UseConsumeFilter(typeof(ConsumerInterceptor<>), context);
            e.ConfigureConsumer<TConsumer>(context);
            e.PrefetchCount = options.PrefetchCount;
            e.ConcurrentMessageLimit = options.ConcurrencyLimit;
            e.AutoOffsetReset = AutoOffsetReset.Earliest;
            e.CheckpointInterval = TimeSpan.FromSeconds(options.CheckpointInterval);
            e.CheckpointMessageCount = (ushort)options.CheckpointMessageCount;
        });
    }
}
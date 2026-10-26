using Fusion.Messaging.RabbitMq.Events;
using Fusion.Messaging.RabbitMq.Handlers;
using Fusion.Messaging.RabbitMq.Integration.Configuration;
using Fusion.Messaging.RabbitMq.Integration.Interceptors;
using Fusion.Messaging.RabbitMq.Internals;
using Fusion.Messaging.RabbitMq.Internals.MassTransit;
using Fusion.Messaging.RabbitMq.Registration.Configurators;
using Fusion.Microservice.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.Messaging.RabbitMq.Registration;

/// <summary>
/// Provides extension methods for configuring RabbitMQ message handling in an application using Wolverine.
/// </summary>
public static class MessagingRegistration
{
    /// <summary>
    /// Configures RabbitMQ messaging for the application using MassTransit and Wolverine.
    /// Adds integration event publishing and custom event consumers.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    /// <param name="integrationSetup">Optional action for configuring integration event handling.</param>
    /// <param name="customEventsSetup">Optional action for configuring custom event consumers.</param>
    public static void AddMessaging(
        this WebApplicationBuilder builder,
        Action<IntegrationSetupConfigurator>? integrationSetup = null,
        Action<CustomEventsConfigurator>? customEventsSetup = null
    )
    {
        var services = builder.Services;

        var integrationConfigurator = new IntegrationSetupConfigurator();
        // Generate setup of related to integration events.
        integrationSetup?.Invoke(integrationConfigurator);

        // Register all the handlers configured by service
        // to consumer-related entity integration event from other services.
        integrationConfigurator.GetHandlerTypes()
            .Values
            .ToList()
            .ForEach(handlerType => services.AddTransient(handlerType));

        // Register the handler strategy and setup of integration configured before by service.
        services.AddSingleton(integrationConfigurator);
        services.AddScoped<IEventHandlerStrategy, EventHandlerStrategy>();

        // If something related to integration configured by service should add
        // and register interceptor of publishing integration events.
        if (integrationConfigurator.Integration is not null)
        {
            services.AddSingleton(integrationConfigurator.Integration);
            services.AddScoped<IntegrationEventPublishingInterceptor>();
        }

        var customConfigurator = new CustomEventsConfigurator();
        // Every custom event registered here.
        customEventsSetup?.Invoke(customConfigurator);

        var serviceOptions = builder.Configuration.GetServiceOptions();
        var rabbitMqOptions = serviceOptions.RabbitMQOptions;

        services.AddMassTransit(options =>
        {
            var massTransitConfigurator = new MassTransitRegistrationConfigurator(options);

            // Register default event consumer in MassTransit. 
            massTransitConfigurator.AddConsumer<EventConsumer>();

            // Register custom event consumers in MassTransit.
            foreach (var consumerType in customConfigurator.ConsumerTypes)
            {
                massTransitConfigurator.AddConsumer(consumerType);
            }

            options.UsingRabbitMq((context, cfg) =>
            {
                var configurator = new MassTransitBusFactoryConfigurator(cfg);
                configurator.ConfigureHost(rabbitMqOptions.Host, rabbitMqOptions.Username, rabbitMqOptions.Password);
                configurator.ConfigureRetry();

                // Configure a default service exchange for publishing integration events.
                configurator.ConfigureDefaultExchange(serviceOptions.ServiceName);

                var massTransitContext = new MassTransitRegistrationContext(context);
                // Register the default inbound queue for the incoming queue.
                // Check to not consume if there is not any external service.
                configurator.ConfigureInboundConsumer(
                    massTransitContext,
                    builder.Configuration,
                    integrationConfigurator.ExternalServices
                );

                // Configure custom events in MassTransit.
                foreach (var action in customConfigurator.ConfigActions)
                {
                    action(massTransitContext, configurator);
                }
            });
        });

        // Register a publisher for events.
        services.AddSingleton<IEventPublisher, EventPublisher>();
    }
}
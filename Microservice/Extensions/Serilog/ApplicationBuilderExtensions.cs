using Fusion.Exceptions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace Fusion.Microservice.Extensions.Serilog;

internal static class ApplicationBuilderExtensions
{
    public static void UseCustomSerilog(this Microsoft.AspNetCore.Builder.WebApplicationBuilder builder)
    {
        var option = builder.Configuration.GetServiceOptions();
        if (option.SeqOptions.IsEnabled && string.IsNullOrEmpty(option.SeqOptions.Endpoint))
            throw new RuntimeException("Seq endpoint is not configured");
        
        builder.Host.UseSerilog((_, lc) =>
        {
            lc.Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithCorrelationIdHeader()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("ServiceName", option.ServiceName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Fatal);
                if(!builder.Environment.IsDevelopment())
                    lc.WriteTo.Console(new ElasticsearchJsonFormatter());
                else
                    lc.WriteTo.Console();
            if (option.SeqOptions.IsEnabled)
                lc.WriteTo.Seq(option.SeqOptions.Endpoint);
        });
    }
}
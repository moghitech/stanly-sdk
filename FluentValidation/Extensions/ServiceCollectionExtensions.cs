using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fusion.FluentValidation.Extensions;

/// <summary>
/// The extension class provides some functionality on IServiceCollection those related with FluentValidation
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers required Fluent validation services &amp; also validators based on introduced type's assemblies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static void AddValidation(this IServiceCollection services, params Type[] types)
    {
        var assemblies = types.Select(x => x.Assembly).ToArray();
        services.AddValidation(assemblies);
    }

    /// <summary>
    /// Registers required Fluent validation services &amp; also validators based on assemblies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static void AddValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetEntryAssembly())
            .AddFluentValidationRulesToSwagger();
    }
}
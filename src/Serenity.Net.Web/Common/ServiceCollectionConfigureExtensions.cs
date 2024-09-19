using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// DI extension methods related to configuration and options
/// </summary>
public static class ServiceCollectionConfigureExtensions
{
    /// <summary>
    /// Calls `Configure&lt;TOptions&gt;` with the section key determined from 
    /// DefaultSectionKeyAttribute on TOptions.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config">The configuration being bound.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureSection<TOptions>(this IServiceCollection services,
        IConfiguration config) where TOptions : class
    {
        return services.Configure<TOptions>((config ?? throw new ArgumentNullException(nameof(config)))
            .GetSection(typeof(TOptions).GetAttribute<DefaultSectionKeyAttribute>()?.SectionKey ??
                throw new ArgumentOutOfRangeException(nameof(TOptions))));
    }


    /// <summary>
    /// Calls `Configure&lt;TOptionsType&gt;` for all setting classes with DefaultSectionKeyAttribute.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config">The configuration being bound.</param>
    /// <param name="typeSource">Type source with setting classes</param>
    /// <param name="predicate">Optional predicate for type filtering</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureSections(this IServiceCollection services,
        IConfiguration config, ITypeSource typeSource, Func<Type, bool> predicate = null)
    {
        var configureExtension = typeof(ServiceCollectionConfigureExtensions)
          .GetMethods(BindingFlags.Static | BindingFlags.Public)
          .Where(x => x.Name == nameof(ConfigureSection) && x.IsGenericMethodDefinition)
          .Where(x => x.GetGenericArguments().Length == 1)
          .Where(x => x.GetParameters().Length == 2)
          .Where(x => x.GetParameters()[0].ParameterType == typeof(IServiceCollection))
          .Where(x => x.GetParameters()[1].ParameterType == typeof(IConfiguration))
          .Single();

        foreach (var type in typeSource.GetTypesWithAttribute(typeof(DefaultSectionKeyAttribute)))
        {
            if (predicate?.Invoke(type) == false)
                continue;

            var configureMethod = configureExtension.MakeGenericMethod(type);
            configureMethod.Invoke(null, [services, config]);
        }

        return services;
    }
}
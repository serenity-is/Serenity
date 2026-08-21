using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Contains helper methods for data script registration.
/// </summary>
public class DataScriptRegistration
{
    /// <summary>
    /// Creates and registers dynamic scripts for types with <see cref="DataScriptAttribute"/>.
    /// </summary>
    /// <param name="scriptManager">The dynamic script manager.</param>
    /// <param name="typeSource">The type source.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="ArgumentNullException">Script manager, type source or service provider is <c>null</c>.</exception>
    public static void RegisterDataScripts(IDynamicScriptManager scriptManager,
        ITypeSource typeSource, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(scriptManager);

        ArgumentNullException.ThrowIfNull(typeSource);

        ArgumentNullException.ThrowIfNull(serviceProvider);

        DataScriptAttribute attr;

        foreach (var type in typeSource.GetTypes())
        {
            if (type.IsAbstract ||
                type.IsInterface ||
                type.IsGenericTypeDefinition ||
                !type.IsPublic)
                continue;

            attr = type.GetCustomAttribute<DataScriptAttribute>();
            if (attr != null)
            {
                var script = (INamedDynamicScript)ActivatorUtilities
                    .CreateInstance(serviceProvider, type);
                scriptManager.Register(script);
                continue;
            }
        }
    }
}

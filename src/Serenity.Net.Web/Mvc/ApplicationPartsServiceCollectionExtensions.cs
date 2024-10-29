using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// DI extension methods related to application part and type source
/// </summary>
public static class ApplicationPartsServiceCollectionExtensions
{
    /// <summary>
    /// Adds an application part type source to the service collection.
    /// Note that this also calls AddMvcCore() to get the part manager if not provided,
    /// and is not found in the collection.
    /// </summary>
    /// <param name="collection">Collection</param>
    /// <param name="partManager">ApplicationPartManager instance.</param>
    /// <param name="topologicalSort">Whether to sort assemblies topologically by references</param>
    public static ApplicationPartsTypeSource AddApplicationPartsTypeSource(this IServiceCollection collection,
        ApplicationPartManager partManager = null, bool topologicalSort = true)
    {
        ArgumentNullException.ThrowIfNull(collection);
        if (GetServiceFromCollection<ITypeSource>(collection) != null)
            ArgumentExceptions.OutOfRange(nameof(collection), "ITypeSource already registered");

        partManager ??= GetServiceFromCollection<ApplicationPartManager>(collection)
            ?? collection.AddMvcCore().PartManager;

        var typeSource = new ApplicationPartsTypeSource(partManager, topologicalSort);
        collection.AddSingleton<ITypeSource>(typeSource);
        return typeSource;
    }

    private static T GetServiceFromCollection<T>(IServiceCollection services)
        where T: class
    {
        return (T)services.LastOrDefault(d => 
            d.ServiceType == typeof(T))?.ImplementationInstance;
    }
}

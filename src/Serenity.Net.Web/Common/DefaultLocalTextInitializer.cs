using Microsoft.AspNetCore.Hosting;
using Serenity.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Default local text initializer
/// </summary>
/// <param name="typeSource">Type source</param>
/// <param name="rowTypeRegistry">Row type registry</param>
/// <param name="webHostEnvironment">Web host environment</param>
public class DefaultLocalTextInitializer(ITypeSource typeSource,
    IRowTypeRegistry rowTypeRegistry = null,
    IWebHostEnvironment webHostEnvironment = null) : ILocalTextInitializer
{
    /// <inheritdoc/>
    public virtual void Initialize(ILocalTextRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry, nameof(registry));

        ServiceCollectionExtensions.AddBaseTexts(registry, typeSource, rowTypeRegistry, includeResources: true);
        AddJsonTexts(registry);
    }

    /// <summary>
    /// Adds json texts to the local text registry
    /// </summary>
    /// <param name="registry">Target registry</param>
    protected virtual void AddJsonTexts(ILocalTextRegistry registry)
    {
        ServiceCollectionExtensions.AddJsonTexts(registry,
            webHostEnvironment?.ContentRootFileProvider, "App_Data/texts", recursive: true);
    }
}
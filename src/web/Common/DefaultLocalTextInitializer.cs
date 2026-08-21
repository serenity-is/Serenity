using Microsoft.AspNetCore.Hosting;
using Serenity.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Default <see cref="ILocalTextInitializer"/> that registers base texts and
/// JSON texts from the <c>App_Data/texts</c> folder.
/// </summary>
/// <param name="typeSource">The type source used to discover text registrations.</param>
/// <param name="rowTypeRegistry">The row type registry used to discover row texts.</param>
/// <param name="webHostEnvironment">The web host environment used to locate the texts folder.</param>
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
    /// Adds JSON texts to the local text registry.
    /// </summary>
    /// <param name="registry">The target registry.</param>
    protected virtual void AddJsonTexts(ILocalTextRegistry registry)
    {
        ServiceCollectionExtensions.AddJsonTexts(registry,
            webHostEnvironment?.ContentRootFileProvider, "App_Data/texts", recursive: true);
    }
}
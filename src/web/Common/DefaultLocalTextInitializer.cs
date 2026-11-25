using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Serenity.Extensions.DependencyInjection;
using Serenity.Localization;

namespace Serenity.Web;

/// <summary>
/// Default <see cref="ILocalTextInitializer"/> that registers base texts and
/// JSON texts from the <c>App_Data/texts</c> folder, then the texts of the <see cref="IDynamicLocalTextSource"/>
/// services, which are kept in the data of the application.
/// </summary>
/// <param name="typeSource">The type source used to discover text registrations.</param>
/// <param name="rowTypeRegistry">The row type registry used to discover row texts.</param>
/// <param name="webHostEnvironment">The web host environment used to locate the texts folder.</param>
/// <param name="dynamicSources">The sources of the texts that are kept in data, if any.</param>
/// <param name="localizationSettings">The localization settings, which give the base language.</param>
public class DefaultLocalTextInitializer(ITypeSource typeSource,
    IRowTypeRegistry? rowTypeRegistry = null,
    IWebHostEnvironment? webHostEnvironment = null,
    IEnumerable<IDynamicLocalTextSource>? dynamicSources = null,
    IOptions<LocalizationSettings>? localizationSettings = null) : ILocalTextInitializer
{
    /// <inheritdoc/>
    public virtual void Initialize(ILocalTextRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry, nameof(registry));

        // before the texts are added, as the texts of the base language are stored as invariant ones
        if (registry is IBaseLanguage baseLanguage)
            baseLanguage.BaseLanguageID = localizationSettings?.Value?.BaseLanguage;

        ServiceCollectionExtensions.AddBaseTexts(registry, typeSource, rowTypeRegistry, includeResources: true);
        AddJsonTexts(registry);
        AddDynamicTexts(registry);
    }

    /// <summary>
    /// Adds the texts of the <see cref="IDynamicLocalTextSource"/> services, after the ones of the texts files, so that they
    /// only add what is missing.
    /// </summary>
    /// <param name="registry">The target registry.</param>
    protected virtual void AddDynamicTexts(ILocalTextRegistry registry)
    {
        foreach (var source in dynamicSources ?? [])
            source.AddTexts(registry);
    }

    /// <summary>
    /// Adds JSON texts to the local text registry.
    /// </summary>
    /// <param name="registry">The target registry.</param>
    protected virtual void AddJsonTexts(ILocalTextRegistry registry)
    {
        if (webHostEnvironment?.ContentRootFileProvider is { } fileProvider)
            ServiceCollectionExtensions.AddJsonTexts(registry, fileProvider, "App_Data/texts", recursive: true);
    }
}
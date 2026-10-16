using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Abstraction for a provider of layout script files declared with
/// <see cref="LayoutScriptFileAttribute"/>.
/// </summary>
public interface ILayoutScriptFileProvider
{
    /// <summary>
    /// Gets the list of app relative (<c>~/</c>) JS file urls to include in the
    /// layout, in the order they should be included, with duplicates removed.
    /// </summary>
    /// <returns>The list of JS file urls.</returns>
    IReadOnlyList<string> GetFiles();
}

/// <summary>
/// Default implementation of <see cref="ILayoutScriptFileProvider"/>, which
/// discovers <see cref="LayoutScriptFileAttribute"/>s from the type source,
/// filters them by the registered <see cref="IFeatureToggles"/> and an optional
/// filter, and returns them ordered with duplicates removed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LayoutScriptFileProvider"/> class.
/// </remarks>
/// <param name="typeSource">The type source.</param>
/// <param name="serviceProvider">The service provider, used to resolve the
/// optional <see cref="IFeatureToggles"/>.</param>
/// <param name="options">The layout script file options.</param>
/// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
public class LayoutScriptFileProvider(ITypeSource typeSource,
    IServiceProvider serviceProvider,
    IOptions<LayoutScriptFileOptions> options) : ILayoutScriptFileProvider
{
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly LayoutScriptFileOptions options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

    /// <inheritdoc/>
    public IReadOnlyList<string> GetFiles()
    {
        var featureToggles = serviceProvider.GetService<IFeatureToggles>();
        var result = new List<(decimal Order, string Path)>();

        foreach (var attr in typeSource.GetAssemblyAttributes<LayoutScriptFileAttribute>())
        {
            if (attr.RequireFeatures?.Length > 0 &&
                featureToggles != null &&
                !featureToggles.IsEnabled(attr.RequireFeatures, attr.RequireAnyFeature))
                continue;

            if (options.Filter != null && !options.Filter(attr))
                continue;

            var path = (attr.Path ?? "").Trim();
            if (path.Length == 0 ||
                !path.StartsWith("~/", StringComparison.Ordinal))
                continue;

            result.Add((attr.Order, path));
        }

        return result
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Path)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

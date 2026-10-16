using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Abstraction for a provider of layout CSS files declared with
/// <see cref="LayoutCssFileAttribute"/>.
/// </summary>
public interface ILayoutCssFileProvider
{
    /// <summary>
    /// Gets the list of app relative (<c>~/</c>) CSS file urls to include in the
    /// layout, in the order they should be included, with duplicates removed.
    /// </summary>
    /// <returns>The list of CSS file urls.</returns>
    IReadOnlyList<string> GetFiles();
}

/// <summary>
/// Default implementation of <see cref="ILayoutCssFileProvider"/>, which
/// discovers <see cref="LayoutCssFileAttribute"/>s from the type source,
/// filters them by the registered <see cref="IFeatureToggles"/> and an optional
/// filter, and returns them ordered with duplicates removed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LayoutCssFileProvider"/> class.
/// </remarks>
/// <param name="typeSource">The type source.</param>
/// <param name="serviceProvider">The service provider, used to resolve the
/// optional <see cref="IFeatureToggles"/>.</param>
/// <param name="options">The layout CSS file options.</param>
/// <exception cref="ArgumentNullException">One of the required arguments is <c>null</c>.</exception>
public class LayoutCssFileProvider(ITypeSource typeSource,
    IServiceProvider serviceProvider,
    IOptions<LayoutCssFileOptions> options) : ILayoutCssFileProvider
{
    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly LayoutCssFileOptions options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

    /// <inheritdoc/>
    public IReadOnlyList<string> GetFiles()
    {
        var featureToggles = serviceProvider.GetService<IFeatureToggles>();
        var result = new List<(decimal Order, string Path)>();

        foreach (var attr in typeSource.GetAssemblyAttributes<LayoutCssFileAttribute>())
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

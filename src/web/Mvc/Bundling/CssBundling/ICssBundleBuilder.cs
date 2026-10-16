namespace Serenity.Web;

/// <summary>
/// Abstraction for building a CSS bundle from a list of source files.
/// </summary>
public interface ICssBundleBuilder
{
    /// <summary>
    /// Builds a dynamic script that concatenates the given CSS source files,
    /// processing <c>@import</c> statements, minimizing and rewriting relative
    /// urls as configured by <see cref="CssBundlingOptions"/>.
    /// </summary>
    /// <param name="rootPrefix">The root prefix used when rewriting relative
    /// urls, e.g. <c>"../"</c> for a bundle served from one level below the
    /// application root.</param>
    /// <param name="sourceFiles">The app relative (<c>~/</c>) CSS file urls or
    /// <c>dynamic://</c> references to other dynamic scripts.</param>
    /// <returns>A dynamic script producing the bundle content.</returns>
    DynamicScript BuildBundle(string rootPrefix, IEnumerable<string> sourceFiles);
}
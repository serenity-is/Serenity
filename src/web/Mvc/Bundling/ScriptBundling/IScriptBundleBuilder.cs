namespace Serenity.Web;

/// <summary>
/// Abstraction for building a script bundle from a list of source files.
/// </summary>
public interface IScriptBundleBuilder
{
    /// <summary>
    /// Builds a dynamic script that concatenates the given JS source files,
    /// minimizing the files as configured by <see cref="ScriptBundlingOptions"/>.
    /// </summary>
    /// <param name="sourceFiles">The app relative (<c>~/</c>) JS file urls or
    /// <c>dynamic://</c> references to other dynamic scripts.</param>
    /// <returns>A dynamic script producing the bundle content.</returns>
    DynamicScript BuildBundle(IEnumerable<string> sourceFiles);
}

using System.Reflection;

namespace Serenity.Extensions;

/// <summary>
/// Base type source for apps using Serenity.Extensions assembly and its reference chain
/// </summary>
/// <param name="assemblies">Additional assemblies to include</param>
public class ExtensionsTypeSource(IEnumerable<Assembly> assemblies)
    : WebTypeSource([])
{
    public override IEnumerable<Assembly> GetAssemblies()
    {
        return SerenityExtensionsAssemblyChain.Concat(assemblies);
    }

    /// <summary>
    /// Reference to Serenity.Extensions assembly
    /// </summary>
    public static readonly Assembly SerenityExtensionsAssembly = typeof(ExtensionsTypeSource).Assembly;

    /// <summary>
    /// List of assemblies up to Serenity.Extensions
    /// </summary>
    public static readonly Assembly[] SerenityExtensionsAssemblyChain =
    [
        ..SerenityNetWebAssemblyChain,
        SerenityExtensionsAssembly
    ];
}
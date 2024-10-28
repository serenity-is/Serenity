namespace Serenity.Extensions;

/// <summary>
/// Base type source for apps using Serenity.Extensions assembly and its reference chain
/// </summary>
/// <param name="assemblies">Additional assemblies to include</param>
public class ExtensionsTypeSource(IEnumerable<Assembly> assemblies)
    : DefaultTypeSource([..SerenityExtensionsAssemblyChain, ..assemblies])
{
    /// <summary>
    /// Reference to Serenity.Net.Core assembly
    /// </summary>
    public static readonly Assembly SerenityNetCoreAssembly = typeof(Localization.ILocalText).Assembly;

    /// <summary>
    /// Reference to Serenity.Net.Data assembly
    /// </summary>
    public static readonly Assembly SerenityNetDataAssembly = typeof(DefaultSqlConnections).Assembly;

    /// <summary>
    /// Reference to Serenity.Net.Entity assembly
    /// </summary>
    public static readonly Assembly SerenityNetEntityAssembly = typeof(Row<>).Assembly;

    /// <summary>
    /// Reference to Serenity.Net.Services assembly
    /// </summary>
    public static readonly Assembly SerenityNetServicesAssembly = typeof(SaveRequestHandler<>).Assembly;

    /// <summary>
    /// Reference to Serenity.Net.Web assembly
    /// </summary>
    public static readonly Assembly SerenityNetWebAssembly = typeof(HttpContextItemsAccessor).Assembly;

    /// <summary>
    /// Reference to Serenity.Extensions assembly
    /// </summary>
    public static readonly Assembly SerenityExtensionsAssembly = typeof(ExtensionsTypeSource).Assembly;

    /// <summary>
    /// List of assemblies up to Serenity.Extensions
    /// </summary>
    public static readonly Assembly[] SerenityExtensionsAssemblyChain =
    [
        SerenityNetCoreAssembly,
        SerenityNetDataAssembly,
        SerenityNetEntityAssembly,
        SerenityNetServicesAssembly,
        SerenityNetWebAssembly,
        SerenityExtensionsAssembly
    ];
}
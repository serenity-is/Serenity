namespace Serenity.Web;

/// <summary>
/// Base type source for apps using Serenity.Web assembly and its reference chain
/// </summary>
public class WebTypeSource(IEnumerable<Assembly> assemblies) : BaseAssemblyTypeSource
{
    private readonly IEnumerable<Assembly> assemblies = assemblies
        ?? throw new ArgumentNullException(nameof(assemblies));

    /// <inheritdoc />
    public override IEnumerable<Assembly> GetAssemblies()
    {
        return SerenityNetWebAssemblyChain.Concat(assemblies);
    }

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
    /// List of assemblies up to Serenity.Web
    /// </summary>
    public static readonly Assembly[] SerenityNetWebAssemblyChain =
    [
        SerenityNetCoreAssembly,
        SerenityNetDataAssembly,
        SerenityNetEntityAssembly,
        SerenityNetServicesAssembly,
        SerenityNetWebAssembly
    ];
}

namespace Serenity.Abstractions;

/// <summary>
/// Default implementation for a type source that accepts an assembly list
/// </summary>
/// <remarks>
/// Creates a new instance
/// </remarks>
/// <param name="assemblies">List of assemblies</param>
public class DefaultTypeSource(IEnumerable<Assembly> assemblies) : BaseAssemblyTypeSource
{
    private readonly IEnumerable<Assembly> assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

    /// <inheritdoc />
    public override IEnumerable<Assembly> GetAssemblies() => assemblies;
}
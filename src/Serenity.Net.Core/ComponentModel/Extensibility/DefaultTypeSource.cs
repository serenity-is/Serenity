
namespace Serenity.Abstractions;

/// <summary>
/// Default implementation for a type source that accepts an assembly list
/// </summary>
/// <remarks>
/// Creates a new instance
/// </remarks>
/// <param name="assemblies">List of assemblies</param>
/// <param name="featureToggles">Feature toggles service used to filter types</param>
public class DefaultTypeSource(IEnumerable<Assembly> assemblies, IFeatureToggles? featureToggles = null) : BaseAssemblyTypeSource(featureToggles)
{
    private readonly IEnumerable<Assembly> assemblies = (assemblies is Array ? assemblies.Distinct().ToArray() : assemblies) 
        ?? throw new ArgumentNullException(nameof(assemblies));

    /// <inheritdoc />
    public override IEnumerable<Assembly> GetAssemblies() => assemblies;
}
namespace Serenity.Abstractions;

/// <summary>
/// Default implementation for a type source
/// </summary>
public class DefaultTypeSource : ITypeSource, IGetAssemblies
{
    private readonly IEnumerable<Assembly> assemblies;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="assemblies">List of assemblies</param>
    public DefaultTypeSource(IEnumerable<Assembly> assemblies)
    {
        this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }

    /// <summary>
    /// Gets all attributes for assemblies with given type
    /// </summary>
    /// <returns>List of attributes for assemblies</returns>
    public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
    {
        return assemblies.SelectMany(x => x.GetCustomAttributes(attributeType));
    }

    /// <summary>
    /// Gets all types
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Type> GetTypes()
    {
        return assemblies.SelectMany(x => x.GetTypes());
    }

    /// <summary>
    /// Gets all types that implement an interface
    /// </summary>
    /// <param name="interfaceType">Interface type</param>
    /// <returns>Types with that interface type</returns>
    public IEnumerable<Type> GetTypesWithInterface(Type interfaceType)
    {
        return assemblies.SelectMany(asm => asm.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type));
    }

    /// <summary>
    /// Gets all types that has an attribute
    /// </summary>
    /// <param name="attributeType">Attribute type</param>
    /// <returns>Types with that attribute type</returns>
    public IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
    {
        return assemblies.SelectMany(asm => asm.GetTypes())
            .Where(type => type.GetCustomAttribute(attributeType) != null);
    }

    /// <inheritdoc/>
    public IEnumerable<Assembly> GetAssemblies()
    {
        return assemblies;
    }
}
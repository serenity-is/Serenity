namespace Serenity.Abstractions;

/// <summary>
/// Base type source implementation that accepts a list of assemblies
/// </summary>
/// <remarks>
/// Creates a new instance
/// </remarks>
public abstract class BaseAssemblyTypeSource() : ITypeSource, IGetAssemblies
{
    /// <summary>
    /// Gets all attributes for assemblies with given type
    /// </summary>
    /// <returns>List of attributes for assemblies</returns>
    public virtual IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
    {
        return GetAssemblies().SelectMany(x => x.GetCustomAttributes(attributeType));
    }

    /// <summary>
    /// Gets all types
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<Type> GetTypes()
    {
        return GetAssemblies().SelectMany(x => x.GetTypes());
    }

    /// <summary>
    /// Gets all types that implement an interface
    /// </summary>
    /// <param name="interfaceType">Interface type</param>
    /// <returns>Types with that interface type</returns>
    public virtual IEnumerable<Type> GetTypesWithInterface(Type interfaceType)
    {
        return GetAssemblies().SelectMany(asm => asm.GetTypes().Where(interfaceType.IsAssignableFrom));
    }

    /// <summary>
    /// Gets all types that has an attribute
    /// </summary>
    /// <param name="attributeType">Attribute type</param>
    /// <returns>Types with that attribute type</returns>
    public virtual IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
    {
        return GetAssemblies().SelectMany(asm => asm.GetTypes()
            .Where(type => type.GetCustomAttribute(attributeType) != null));
    }

    /// <inheritdoc/>
    public abstract IEnumerable<Assembly> GetAssemblies();
}
namespace Serenity.Abstractions;

/// <summary>
/// Abstraction for type source which is used as an extensibility point
/// for Serenity applications
/// </summary>
public interface ITypeSource
{
    /// <summary>
    /// Gets all attributes for assemblies with given type
    /// </summary>
    /// <returns>List of attributes for assemblies</returns>
    IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType);
    /// <summary>
    /// Gets all types
    /// </summary>
    /// <returns></returns>
    IEnumerable<Type> GetTypes();
    /// <summary>
    /// Gets all types that implement an interface
    /// </summary>
    /// <param name="interfaceType">Interface type</param>
    /// <returns>Types with that interface type</returns>
    IEnumerable<Type> GetTypesWithInterface(Type interfaceType);
    /// <summary>
    /// Gets all types that has an attribute
    /// </summary>
    /// <param name="attributeType">Attribute type</param>
    /// <returns>Types with that attribute type</returns>
    IEnumerable<Type> GetTypesWithAttribute(Type attributeType);
}
namespace Serenity.Reflection;

/// <summary>
/// An interface to virtualize property attribute access
/// </summary>
public interface IPropertyInfo
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    string Name { get; }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    TAttr? GetAttribute<TAttr>() where TAttr : Attribute;

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute;

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    /// <value>
    /// The type of the property.
    /// </value>
    Type PropertyType { get; }
}
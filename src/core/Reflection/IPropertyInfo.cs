namespace Serenity.Reflection;

/// <summary>
/// An interface to virtualize property attribute access.
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
    /// Gets the attribute of the specified type.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <param name="origin">The attribute origin to search.</param>
    /// <returns>The attribute of the specified type, or <c>null</c> if none is found.</returns>
    TAttr? GetAttribute<TAttr>(AttributeOrigin origin = AttributeOrigin.All) where TAttr : Attribute;

    /// <summary>
    /// Gets the attributes of the specified type.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <param name="origin">The attribute origin to search.</param>
    /// <returns>The attributes of the specified type.</returns>
    IEnumerable<TAttr> GetAttributes<TAttr>(AttributeOrigin origin = AttributeOrigin.All) where TAttr : Attribute;

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    /// <value>
    /// The type of the property.
    /// </value>
    Type PropertyType { get; }
}
using Serenity.Reflection;

namespace Serenity.PropertyGrid;

/// <summary>
/// Abstracts the property source that returns PropertyInfo and some other info
/// </summary>
public interface IPropertySource : IPropertyInfo
{
    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    PropertyInfo Property { get; }
    /// <summary>
    /// Gets the based on field.
    /// </summary>
    /// <value>
    /// The based on field.
    /// </value>
    Field BasedOnField { get; }
    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    Type ValueType { get; }
    /// <summary>
    /// Gets the type of the enum.
    /// </summary>
    /// <value>
    /// The type of the enum.
    /// </value>
    Type EnumType { get; }
}
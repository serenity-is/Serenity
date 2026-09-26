namespace Serenity.Data.Mapping;

/// <summary>
/// Determines the foreign key property on this row for a foreign row property. 
/// This is used to locate the FK property.
/// </summary>
/// <param name="value">The name of the FK property on this row.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ForeignRowAttribute(string value) : Attribute
{
    /// <summary>
    /// Gets the FK property name.
    /// </summary>
    /// <value>
    /// The FK property name.
    /// </value>
    public string ForeignKeyProperty { get; } = string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException("A foreign key property name is required.", nameof(value))
        : value;
}
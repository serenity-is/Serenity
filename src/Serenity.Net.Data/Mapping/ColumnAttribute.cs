namespace Serenity.Data.Mapping;

/// <summary>
/// Explicitly specifies the database column name for property.
/// Use this attribute if matching column name in database is different than the property name.
/// </summary>
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <exception cref="ArgumentNullException">name</exception>
    public ColumnAttribute(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");

        Name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; private set; }
}
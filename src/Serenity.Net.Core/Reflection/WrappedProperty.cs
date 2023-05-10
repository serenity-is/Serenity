namespace Serenity.Reflection;

/// <summary>
/// A class that basically implements IPropertyInfo for PropertyInfo objects
/// </summary>
public class WrappedProperty : IPropertyInfo
{
    private readonly PropertyInfo property;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedProperty"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    public WrappedProperty(PropertyInfo property)
    {
        this.property = property;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name => property.Name;

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    /// <value>
    /// The type of the property.
    /// </value>
    public Type PropertyType => property.PropertyType;

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    public TAttr GetAttribute<TAttr>() where TAttr : Attribute
    {
        return property.GetCustomAttribute<TAttr>();
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    public IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute
    {
        return property.GetCustomAttributes<TAttr>();
    }
}
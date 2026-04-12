namespace Serenity.Reflection;

/// <summary>
/// A class that basically implements IPropertyInfo for PropertyInfo objects
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WrappedProperty"/> class.
/// </remarks>
/// <param name="property">The property.</param>
public class WrappedProperty(PropertyInfo property) : IPropertyInfo
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo?> providerAttributesPropertyByType = new();

    private readonly PropertyInfo property = property;
    private Attribute[]? cachedAttributes;

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
    public TAttr? GetAttribute<TAttr>() where TAttr : Attribute
    {
        TAttr? result = null;
        foreach (var attr in GetCachedAttributes())
            if (attr is TAttr typed)
            {
                if (result is not null)
                    throw new AmbiguousMatchException(string.Format("Property {0} has multiple attributes of type {1}", Name, typeof(TAttr).FullName));

                result = typed;
            }

        return result;
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <returns></returns>
    public IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute
    {
        foreach (var attr in GetCachedAttributes())
            if (attr is TAttr typed)
                yield return typed;
    }

    private Attribute[] GetCachedAttributes()
    {
        var cachedAttributes = this.cachedAttributes;
        if (cachedAttributes is not null)
            return cachedAttributes;
         
        var directAttributes = property.GetCustomAttributes<Attribute>();
        var allAttributes = new List<Attribute>(directAttributes);

        foreach (var customAttr in directAttributes)
        {
            if (customAttr is not IIntrinsicPropertyAttributeProvider)
                continue;

            var providerAttributesProperty = providerAttributesPropertyByType.GetOrAdd(customAttr.GetType(), static t =>
                t.GetProperty(nameof(IIntrinsicPropertyAttributeProvider.PropertyAttributes),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

            if (providerAttributesProperty == null)
                continue;

            allAttributes.AddRange(providerAttributesProperty.GetCustomAttributes<Attribute>());
        }

        cachedAttributes = [.. allAttributes];

        return this.cachedAttributes = cachedAttributes;
    }
}
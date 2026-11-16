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
    private static readonly ConcurrentDictionary<PropertyInfo, PropertyInfo[]> interfacePropertiesByProperty = new();
    private static readonly ConcurrentDictionary<Type, bool> allowMultipleByType = new();

    private readonly PropertyInfo property = property;
    // cache slots indexed by (inherit ? 2 : 0) | (intrinsic ? 1 : 0)
    private readonly Attribute[]?[] cachedAttributesByOrigin = new Attribute[]?[4];

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
    /// <param name="origin">The attribute origin to search.</param>
    /// <returns>The attribute of the specified type, or <c>null</c> if none is found.</returns>
    public TAttr? GetAttribute<TAttr>(AttributeOrigin origin = AttributeOrigin.All) where TAttr : Attribute
    {
        TAttr? result = null;
        foreach (var attr in GetCachedAttributes(origin))
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
    /// <param name="origin">The attribute origin to search.</param>
    /// <returns>The attributes of the specified type.</returns>
    public IEnumerable<TAttr> GetAttributes<TAttr>(AttributeOrigin origin = AttributeOrigin.All) where TAttr : Attribute
    {
        foreach (var attr in GetCachedAttributes(origin))
            if (attr is TAttr typed)
                yield return typed;
    }

    private Attribute[] GetCachedAttributes(AttributeOrigin origin)
    {
        var index = (origin.HasFlag(AttributeOrigin.Inherit) ? 2 : 0) |
            (origin.HasFlag(AttributeOrigin.Intrinsic) ? 1 : 0);

        var cachedAttributes = cachedAttributesByOrigin[index];
        if (cachedAttributes is not null)
            return cachedAttributes;

        bool inherit = (index & 2) != 0;
        bool intrinsic = (index & 1) != 0;

        var attributes = GetAttributesOf(property, inherit, intrinsic);

        // the attributes of the members of the interfaces the property implements are inherited like the ones of the
        // base classes, so one of a type that doesn't allow multiple is only used when the property doesn't have one itself
        if (inherit && GetImplementedInterfaceProperties(property) is { Length: > 0 } interfaceProperties)
        {
            var ownTypes = new HashSet<Type>(attributes.Select(x => x.GetType()));
            var merged = new List<Attribute>(attributes);
            foreach (var interfaceProperty in interfaceProperties)
            {
                foreach (var attr in GetAttributesOf(interfaceProperty, inherit: false, intrinsic))
                {
                    var type = attr.GetType();
                    if (!ownTypes.Contains(type) || AllowsMultiple(type))
                        merged.Add(attr);
                }
            }

            attributes = [.. merged];
        }

        return cachedAttributesByOrigin[index] = attributes;
    }

    internal static bool AllowsMultiple(Type attributeType)
    {
        return allowMultipleByType.GetOrAdd(attributeType, static type =>
            type.GetCustomAttribute<AttributeUsageAttribute>(inherit: true)?.AllowMultiple ?? false);
    }

    private static Attribute[] GetAttributesOf(PropertyInfo property, bool inherit, bool intrinsic)
    {
        var directAttributes = property.GetCustomAttributes<Attribute>(inherit);
        var allAttributes = new List<Attribute>(directAttributes);

        // intrinsic provider attributes are only included when the Intrinsic origin
        // bit is requested, so e.g. AttributeOrigin.Explicit does not leak them.
        if (intrinsic)
        {
            foreach (var customAttr in directAttributes)
            {
                if (customAttr is not IIntrinsicPropertyAttributeProvider)
                    continue;

                var providerAttributesProperty = providerAttributesPropertyByType.GetOrAdd(customAttr.GetType(), static t =>
                    t.GetProperty(nameof(IIntrinsicPropertyAttributeProvider.PropertyAttributes),
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

                if (providerAttributesProperty == null)
                    continue;

                allAttributes.AddRange(providerAttributesProperty.GetCustomAttributes<Attribute>(inherit: false));
            }
        }

        return [.. allAttributes];
    }

    /// <summary>
    /// Gets the properties of the interfaces that a property implements, in the type it was obtained from, ordered by the full
    /// names of the interfaces, so the result doesn't depend on the order the runtime lists them in.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The properties of the interfaces, or an empty array when it implements none.</returns>
    public static PropertyInfo[] GetImplementedInterfaceProperties(PropertyInfo property)
    {
        if (property is null)
            throw new ArgumentNullException(nameof(property));

        return interfacePropertiesByProperty.GetOrAdd(property, static property =>
        {
            var type = property.ReflectedType;
            var accessor = property.GetMethod ?? property.SetMethod;
            if (type is null || type.IsInterface || accessor is null || accessor.IsStatic)
                return [];

            var result = new List<PropertyInfo>();
            foreach (var interfaceType in type.GetInterfaces().OrderBy(x => x.FullName ?? x.Name, StringComparer.Ordinal))
            {
                InterfaceMapping map;
                try
                {
                    map = type.GetInterfaceMap(interfaceType);
                }
                catch (ArgumentException)
                {
                    // e.g. a generic type definition, which has no mapping
                    continue;
                }

                for (var i = 0; i < map.TargetMethods.Length; i++)
                {
                    var target = map.TargetMethods[i];
                    if (target is null ||
                        target.MetadataToken != accessor.MetadataToken ||
                        target.Module != accessor.Module ||
                        target.DeclaringType != accessor.DeclaringType)
                        continue;

                    var interfaceMethod = map.InterfaceMethods[i];
                    foreach (var interfaceProperty in interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if ((interfaceProperty.GetMethod == interfaceMethod || interfaceProperty.SetMethod == interfaceMethod) &&
                            !result.Contains(interfaceProperty))
                            result.Add(interfaceProperty);
                    }
                }
            }

            return [.. result];
        });
    }
}

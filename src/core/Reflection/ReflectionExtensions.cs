namespace Serenity;

/// <summary>
/// Reflection extensions.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the attribute. Obsolete, use GetCustomAttribute(bool)"/> instead.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="member">The member.</param>
    /// <param name="inherit">if set to <c>true</c>, searches the inheritance chain.</param>
    [Obsolete]
    public static TAttribute? GetAttribute<TAttribute>(this MemberInfo member, bool inherit = false) where TAttribute : Attribute
    {
        return member.GetCustomAttribute<TAttribute>(inherit);
    }

    /// <summary>
    /// Gets an attribute of a property as Serenity resolves it: its own, the ones it inherits from its base classes and from the
    /// members of the interfaces it implements, and the ones of its combination attributes, see
    /// <see cref="Reflection.IIntrinsicPropertyAttributeProvider"/>, as <see cref="Reflection.WrappedProperty"/> does. Unlike
    /// <see cref="CustomAttributeExtensions.GetCustomAttribute{T}(MemberInfo)"/>, which only has its own and the inherited ones.
    /// </summary>
    /// <remarks>
    /// Only the attributes of the type, and the combination attributes, are created, so it can be used where creating the
    /// others would recurse, e.g. in the constructor of an attribute that reads the properties of another type, which can have
    /// the same attribute.
    /// </remarks>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="property">The property.</param>
    /// <returns>The attribute, or null when the property has none.</returns>
    /// <exception cref="AmbiguousMatchException">The property has more than one attribute of the type.</exception>
    public static TAttribute? GetPropertyAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
    {
        TAttribute? result = null;
        foreach (var attr in GetPropertyAttributes<TAttribute>(property))
        {
            if (result is not null)
                throw new AmbiguousMatchException(string.Format("Property {0} has multiple attributes of type {1}",
                    property.Name, typeof(TAttribute).FullName));

            result = attr;
        }

        return result;
    }

    /// <summary>
    /// Gets the attributes of a property as Serenity resolves them, see <see cref="GetPropertyAttribute{TAttribute}(PropertyInfo)"/>.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attributes.</typeparam>
    /// <param name="property">The property.</param>
    /// <returns>The attributes.</returns>
    public static IEnumerable<TAttribute> GetPropertyAttributes<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
    {
        if (property is null)
            throw new ArgumentNullException(nameof(property));

        var own = GetOwnAndIntrinsicAttributes<TAttribute>(property, inherit: true);
        var interfaceProperties = Reflection.WrappedProperty.GetImplementedInterfaceProperties(property);
        if (interfaceProperties.Length == 0)
            return own;

        // the ones of the interfaces are inherited like the ones of the base classes, see WrappedProperty
        var ownTypes = new HashSet<Type>(own.Select(x => x.GetType()));
        foreach (var interfaceProperty in interfaceProperties)
        {
            foreach (var attr in GetOwnAndIntrinsicAttributes<TAttribute>(interfaceProperty, inherit: false))
            {
                var type = attr.GetType();
                if (!ownTypes.Contains(type) || Reflection.WrappedProperty.AllowsMultiple(type))
                    own.Add(attr);
            }
        }

        return own;
    }

    private static List<TAttribute> GetOwnAndIntrinsicAttributes<TAttribute>(PropertyInfo property, bool inherit)
        where TAttribute : Attribute
    {
        var result = property.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToList();
        var ownTypes = new HashSet<Type>(result.Select(x => x.GetType()));

        // the attributes a combination attribute carries are on the PropertyAttributes property of its type, so it is found
        // without being created, as its constructor could read the attributes of a row too, e.g. one deriving from ForeignKey
        foreach (var providerType in GetIntrinsicProviderTypes(property, inherit))
        {
            var attributesProperty = providerType.GetProperty(nameof(Reflection.IIntrinsicPropertyAttributeProvider.PropertyAttributes),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (attributesProperty is not null)
            {
                // an attribute of the property itself wins over the one of the same type a combination attribute has
                foreach (var attr in attributesProperty.GetCustomAttributes(typeof(TAttribute), inherit: false).Cast<TAttribute>())
                {
                    var type = attr.GetType();
                    if (!ownTypes.Contains(type) || Reflection.WrappedProperty.AllowsMultiple(type))
                        result.Add(attr);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the types of the combination attributes of a property, from the data of its attributes, so none is created. With
    /// inherit, the ones of the properties it overrides are included like GetCustomAttributes includes them: only the ones whose
    /// usage is inherited, and of a type that allows multiple, or that a property overriding it doesn't have already.
    /// </summary>
    private static List<Type> GetIntrinsicProviderTypes(PropertyInfo property, bool inherit)
    {
        var result = new List<Type>();
        var level = 0;
        for (var current = property; current is not null; current = inherit ? GetOverriddenProperty(current) : null, level++)
        {
            foreach (var data in current.CustomAttributes)
            {
                var type = data.AttributeType;
                if (!typeof(Reflection.IIntrinsicPropertyAttributeProvider).IsAssignableFrom(type))
                    continue;

                if (level > 0)
                {
                    var usage = type.GetCustomAttribute<AttributeUsageAttribute>(inherit: true);
                    if (usage?.Inherited == false ||
                        (usage?.AllowMultiple != true && result.Contains(type)))
                        continue;
                }

                result.Add(type);
            }
        }

        return result;
    }

    private static PropertyInfo? GetOverriddenProperty(PropertyInfo property)
    {
        var accessor = property.GetMethod ?? property.SetMethod;
        // a property that doesn't override another is where its definition is declared
        if (accessor is null || !accessor.IsVirtual || accessor.GetBaseDefinition().DeclaringType == accessor.DeclaringType)
            return null;

        for (var type = property.DeclaringType?.BaseType; type is not null; type = type.BaseType)
        {
            var baseProperty = type.GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (baseProperty is not null)
                return baseProperty;
        }

        return null;
    }
}

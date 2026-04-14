using Serenity.Reflection;

namespace Serenity.PropertyGrid;

/// <summary>
/// Property info source for a reflection PropertyInfo object
/// </summary>
/// <seealso cref="IPropertySource" />
public class PropertyInfoSource : IPropertySource
{
    private readonly WrappedProperty wrappedProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyInfoSource"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="basedOnRow">The based on row.</param>
    /// <exception cref="ArgumentNullException">property</exception>
    public PropertyInfoSource(PropertyInfo property, IRow basedOnRow)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
        wrappedProperty = new WrappedProperty(property);
        BasedOnRow = basedOnRow;

        if (basedOnRow != null)
        {
            BasedOnField = basedOnRow.Fields.FindFieldByPropertyName(property.Name);

            if (BasedOnField is null)
            {
                // only use field found by field name if it has no property
                var byFieldName = basedOnRow.Fields.FindField(property.Name);
                if (byFieldName is not null &&
                    string.IsNullOrEmpty(byFieldName.PropertyName))
                    BasedOnField = byFieldName;
            }
        }

        var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
        ValueType = nullableType ?? property.PropertyType;

        if (ValueType.IsEnum)
            EnumType = ValueType;
        else if (
            BasedOnField is not null
            && BasedOnField is IEnumTypeField)
        {
            EnumType = (BasedOnField as IEnumTypeField).EnumType;
            if (EnumType != null && !EnumType.IsEnum)
                EnumType = null;
        }
    }

    /// <inheritdoc/>
    public TAttribute GetAttribute<TAttribute>(AttributeOrigin origin = AttributeOrigin.All)
        where TAttribute : Attribute
    {
        return wrappedProperty.GetAttribute<TAttribute>(origin) ??
            (origin.HasFlag(AttributeOrigin.BasedOnField) ?
             BasedOnField?.GetAttribute<TAttribute>() : null);
    }

    /// <inheritdoc/>
    public IEnumerable<TAttribute> GetAttributes<TAttribute>(AttributeOrigin origin = AttributeOrigin.All)
        where TAttribute : Attribute
    {
        var attrList = new List<TAttribute>();
        attrList.AddRange(wrappedProperty.GetAttributes<TAttribute>(origin));

        if (origin.HasFlag(AttributeOrigin.BasedOnField) &&
            BasedOnField is not null)
        {
            foreach (var a in BasedOnField.CustomAttributes)
                if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                    attrList.Add((TAttribute)a);
        }

        return attrList;
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    public PropertyInfo Property { get; private set; }

    /// <inheritdoc/>
    public Type ValueType { get; private set; }

    /// <inheritdoc/>
    public Type EnumType { get; private set; }

    /// <inheritdoc/>
    public IRow BasedOnRow { get; private set; }

    /// <inheritdoc/>
    public Field BasedOnField { get; private set; }

    /// <inheritdoc/>
    public string Name => Property.Name;

    /// <inheritdoc/>
    public Type PropertyType => Property.PropertyType;

    /// <inheritdoc/>
    public Type ReflectedType => Property.ReflectedType;
}
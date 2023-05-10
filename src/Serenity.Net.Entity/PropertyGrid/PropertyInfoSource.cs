namespace Serenity.PropertyGrid;

/// <summary>
/// Property info source for a reflection PropertyInfo object
/// </summary>
/// <seealso cref="IPropertySource" />
public class PropertyInfoSource : IPropertySource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyInfoSource"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="basedOnRow">The based on row.</param>
    /// <exception cref="ArgumentNullException">property</exception>
    public PropertyInfoSource(PropertyInfo property, IRow basedOnRow)
    {
        Property = property ?? throw new ArgumentNullException("property");
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

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns></returns>
    public TAttribute GetAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return Property.GetCustomAttribute<TAttribute>() ??
            BasedOnField.GetAttribute<TAttribute>();
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns></returns>
    public IEnumerable<TAttribute> GetAttributes<TAttribute>()
        where TAttribute : Attribute
    {
        var attrList = new List<TAttribute>();
        attrList.AddRange(Property.GetCustomAttributes<TAttribute>());

        if (BasedOnField is not null)
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
    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public Type ValueType { get; private set; }
    /// <summary>
    /// Gets the type of the enum.
    /// </summary>
    /// <value>
    /// The type of the enum.
    /// </value>
    public Type EnumType { get; private set; }
    /// <summary>
    /// Gets the based on row.
    /// </summary>
    /// <value>
    /// The based on row.
    /// </value>
    public IRow BasedOnRow { get; private set; }
    /// <summary>
    /// Gets the based on field.
    /// </summary>
    /// <value>
    /// The based on field.
    /// </value>
    public Field BasedOnField { get; private set; }
}
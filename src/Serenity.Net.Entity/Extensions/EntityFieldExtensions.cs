namespace Serenity.Data;

/// <summary>
///   Contains static extension methods for Field objects.</summary>
public static class EntityFieldExtensions
{
    private const FieldFlags NonTableFieldFlags =
        FieldFlags.NotMapped | FieldFlags.Foreign | FieldFlags.Calculated | FieldFlags.Reflective;

    /// <summary>
    /// Checks to see if field is an actual table field, e.g. not a foreign or calculated
    /// field. This is determined by field flags and having expression.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    /// True if field seems to be an actual table field.
    /// </returns>
    /// <exception cref="ArgumentNullException">field</exception>
    public static bool IsTableField(this Field field)
    {
        return field is null ? throw new ArgumentNullException(nameof(field)) : (field.Flags & NonTableFieldFlags) == 0;
    }

    /// <summary>
    ///   Gets a dictionary of table fields (e.g. not a foreign or calculated field) in a row.</summary>
    /// <param name="row">
    ///   The row to return dictionary of table fields</param>
    /// <returns>
    ///   A dictionary of table fields in which field objects are keys.</returns>
    public static IEnumerable<Field> EnumerateTableFields(this IRow row)
    {
        var fields = row.Fields;
        for (int i = 0; i < fields.Count; i++)
        {
            Field field = fields[i];
            if (field.IsTableField())
                yield return field;
        }
    }

    /// <summary>
    ///   Gets a dictionary of table fields (e.g. not a foreign or calculated field) in a row.</summary>
    /// <param name="row">
    ///   The row to return dictionary of table fields</param>
    /// <returns>
    ///   A dictionary of table fields in which field objects are keys.</returns>
    public static HashSet<Field> GetTableFields(this IRow row)
    {
        HashSet<Field> tableFields = new HashSet<Field>();
        var fields = row.Fields;

        for (int i = 0; i < fields.Count; i++)
        {
            Field field = fields[i];
            if (IsTableField(field))
                tableFields.Add(field);
        }
        return tableFields;
    }


    /// <summary>
    /// Automatically performs trim on field value based on the field flags
    /// TrimToEmpty and Trim.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="row">The row.</param>
    public static void AutoTrim(this Field field, IRow row)
    {
        if (field as StringField is not null &&
            (field.Flags & FieldFlags.Trim) == FieldFlags.Trim)
        {
            string value = (field as StringField)[row];

            if ((field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                value = value.TrimToEmpty();
            else // TrimToNull
                value = value.TrimToNull();
            (field as StringField)[row] = value;
        }
    }

    /// <summary>
    /// Returns a new field an expression with specified join alias. Avoid using.
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <param name="field">The field.</param>
    /// <param name="join">The join.</param>
    /// <param name="origin">The origin.</param>
    /// <param name="extraFlags">The extra flags.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">join</exception>
    public static TField OfJoin<TField>(this TField field, Join join, string origin, FieldFlags extraFlags = FieldFlags.Internal)
        where TField : Field
    {
        if (join == null)
            throw new ArgumentNullException("join");

        field.Expression = join.Name + "." + origin;

        if (field.Flags == FieldFlags.Default)
            field.Flags = FieldFlags.Foreign | extraFlags;
        else
            field.Flags = field.Flags | FieldFlags.Foreign | extraFlags;

        return field;
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="field">The field.</param>
    /// <returns>First attribute with specified type.</returns>
    public static TAttribute GetAttribute<TAttribute>(this Field field)
        where TAttribute : Attribute
    {
        if (field is null)
            return null;

        foreach (var a in field.CustomAttributes)
            if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                return (TAttribute)a;

        return null;
    }


    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="field">The field.</param>
    /// <returns>Attributes with specified type.</returns>
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Field field)
        where TAttribute : Attribute
    {
        if (field is null || field.CustomAttributes.Length == 0)
            yield break;

        foreach (var a in field.CustomAttributes)
            if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                yield return (TAttribute)a;

        yield break;
    }
}
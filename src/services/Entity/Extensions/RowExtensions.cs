namespace Serenity.Data;

/// <summary>
/// Contains extensions for row objects.
/// </summary>
public static class RowExtensions
{
    /// <summary>
    /// Clones the specified row.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="row">The row.</param>
    /// <returns>A clone of the row.</returns>
    public static TRow Clone<TRow>(this TRow row) where TRow : IRow
    {
        return (TRow)row.CloneRow();
    }

    /// <summary>
    /// Applies the default values.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="row">The row.</param>
    /// <param name="unassignedOnly">if set to <c>true</c>, only unassigned fields are set.</param>
    /// <returns>The row with default values applied.</returns>
    /// <exception cref="ArgumentNullException">row is null.</exception>
    public static TRow ApplyDefaultValues<TRow>(this TRow row, bool unassignedOnly = false)
        where TRow : IRow
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        foreach (var field in row.Fields)
        {
            if (unassignedOnly && row.IsAssigned(field))
                continue;

            var value = field.DefaultValue;
            if (value != null)
                field.AsInvariant(row, value);
        }

        return row;
    }

    /// <summary>
    /// Finds a field by its column or field name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The column or field name.</param>
    /// <returns>The matching field, or <see langword="null"/> if no field has that name.</returns>
    /// <remarks>
    /// This lookup does not search by C# property name. Use <see cref="FindFieldByPropertyName"/> for a
    /// property-name lookup, or <see cref="FindFieldByPropertyOrFieldName"/> when the name may be a property
    /// name or any column or field name.
    /// </remarks>
    public static Field? FindField(this IRow row, string? name)
    {
        return name == null ? null : row.Fields.FindField(name);
    }

    /// <summary>
    /// Finds a field by its C# property name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The C# property name.</param>
    /// <returns>The matching field, or <see langword="null"/> if no field has that property.</returns>
    /// <remarks>
    /// This lookup does not fall back to a column or field name. Use
    /// <see cref="FindFieldByPropertyOrFieldName"/> to try the property name first and then fall back to any
    /// column or field name, or <see cref="FindFieldByPublicName"/> to restrict that fallback to fields without
    /// a corresponding property.
    /// </remarks>
    public static Field? FindFieldByPropertyName(this IRow row, string? name)
    {
        return row.Fields.FindFieldByPropertyName(name);
    }

    /// <summary>
    /// Finds a field by its public name: its C# property name, or its field name when it has no corresponding property.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">A C# property name, or the field name of a field without a corresponding property.</param>
    /// <returns>The matching field, or <see langword="null"/> if no eligible field matches.</returns>
    /// <remarks>
    /// Property-name matches take precedence. A column or field name is accepted only if the matching field has no
    /// corresponding C# property name. Use <see cref="FindFieldByPropertyOrFieldName"/> to also allow column or
    /// field names for property-backed fields.
    /// </remarks>
    public static Field? FindFieldByPublicName(this IRow row, string? name)
    {
        return row.Fields.FindFieldByPublicName(name);
    }

    /// <summary>
    /// Finds a field by its C# property name, falling back to its column or field name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The C# property name or the column or field name.</param>
    /// <returns>The matching field, or <see langword="null"/> if no field matches.</returns>
    /// <remarks>
    /// A property-name match takes precedence. If no property matches, this lookup accepts a column or field name
    /// even when the matching field has a corresponding C# property. Use <see cref="FindFieldByPublicName"/> to
    /// restrict that fallback to fields without a corresponding property.
    /// </remarks>
    public static Field? FindFieldByPropertyOrFieldName(this IRow row, string? name)
    {
        return row.Fields.FindFieldByPropertyOrFieldName(name);
    }

    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>The fields of the row.</returns>
    public static RowFieldsBase GetFields(this IRow row)
    {
        return row.Fields;
    }

    /// <summary>
    /// Returns Row's IdField. If row's IdField is null it throws.
    /// </summary>
    public static Field GetIdField(this IRow row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.IdField is not Field field)
            throw new InvalidOperationException($"{row.GetType().FullName}'s IdField is null!");

        return field;
    }
}
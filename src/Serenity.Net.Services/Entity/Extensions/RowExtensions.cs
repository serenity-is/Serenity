namespace Serenity.Data;

/// <summary>
/// Contains extensions for row objects
/// </summary>
public static class RowExtensions
{
    /// <summary>
    /// Clones the specified row.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public static TRow Clone<TRow>(this TRow row) where TRow : IRow
    {
        return (TRow)(row.CloneRow());
    }

    /// <summary>
    /// Applies the default values.
    /// </summary>
    /// <typeparam name="TRow">The type of the row.</typeparam>
    /// <param name="row">The row.</param>
    /// <param name="unassignedOnly">if set to <c>true</c> [unassigned only].</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">row</exception>
    public static TRow ApplyDefaultValues<TRow>(this TRow row, bool unassignedOnly = false)
        where TRow : IRow
    {
        if (row == null)
            throw new ArgumentNullException("row");

        foreach (var field in row.Fields)
        {
            if (unassignedOnly && row.IsAssigned(field))
                continue;

            var value = field.DefaultValue;
            if (value != null)
                field.AsObject(row, field.ConvertValue(value, CultureInfo.InvariantCulture));
        }

        return row;
    }

    /// <summary>
    /// Finds the field by its name
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static Field FindField(this IRow row, string name)
    {
        return row.Fields.FindField(name);
    }

    /// <summary>
    /// Finds the field by property name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static Field FindFieldByPropertyName(this IRow row, string name)
    {
        return row.Fields.FindFieldByPropertyName(name);
    }

    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public static RowFieldsBase GetFields(this IRow row)
    {
        return row.Fields;
    }
}
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
        return (TRow)(row.CloneRow());
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
    /// Finds the field by its name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The name.</param>
    /// <returns>The field with the specified name.</returns>
    public static Field? FindField(this IRow row, string? name)
    {
        return name == null ? null : row.Fields.FindField(name);
    }

    /// <summary>
    /// Finds the field by its property name.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="name">The name.</param>
    /// <returns>The field with the specified property name.</returns>
    public static Field? FindFieldByPropertyName(this IRow row, string? name)
    {
        return row.Fields.FindFieldByPropertyName(name);
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
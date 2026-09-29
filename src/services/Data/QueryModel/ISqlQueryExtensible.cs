namespace Serenity.Data;

/// <summary>
/// Extensible SQL query interface. Used to abstract Serenity.Data.Row dependency from SqlQuery.
/// </summary>
public interface ISqlQueryExtensible
{
    /// <summary>
    /// Gets the into rows.
    /// </summary>
    /// <value>
    /// The into rows.
    /// </value>
    IReadOnlyList<object> IntoRows { get; }

    /// <summary>
    /// Gets the currently selected into row.
    /// </summary>
    object? CurrentIntoRow { get; }

    /// <summary>
    /// Selects the into row.
    /// </summary>
    /// <param name="into">The into row.</param>
    void IntoRowSelection(object? into);

    /// <summary>
    /// Gets the first into row.
    /// </summary>
    /// <value>
    /// The first into row, or <c>null</c> if none.
    /// </value>
    object? FirstIntoRow { get; }

    /// <summary>
    /// Gets the columns.
    /// </summary>
    /// <value>
    /// Read-only view of the columns.
    /// </value>
    IReadOnlyList<SqlQuery.Column> Columns { get; }

    /// <summary>
    /// Gets the index of the select into.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>The index of the select into field, or -1 if not found.</returns>
    int GetSelectIntoIndex(IField field);

    /// <summary>
    /// Gets the aliased sources added to the FROM clause.
    /// </summary>
    IReadOnlyList<object> FromSources { get; }

    /// <summary>
    /// Gets the sources registered for resolving joins by alias.
    /// </summary>
    IEnumerable<object> JoinSources { get; }
}

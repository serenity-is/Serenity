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
    IList<object> IntoRows { get; }

    /// <summary>
    /// Selects the into row.
    /// </summary>
    /// <param name="into">The into.</param>
    void IntoRowSelection(object into);

    /// <summary>
    /// Gets the first into row.
    /// </summary>
    /// <value>
    /// The first into row.
    /// </value>
    object FirstIntoRow { get; }

    /// <summary>
    /// Gets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    IList<SqlQuery.Column> Columns { get; }

    /// <summary>
    /// Gets the index of the select into.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    int GetSelectIntoIndex(IField field);
}

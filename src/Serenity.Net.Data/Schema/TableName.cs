namespace Serenity.Data.Schema;

/// <summary>
/// Table schema/name and view information
/// </summary>
public class TableName
{
    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    /// <value>
    /// The schema.
    /// </value>
    public string Schema { get; set; }

    /// <summary>
    /// Gets or sets the table.
    /// </summary>
    /// <value>
    /// The table.
    /// </value>
    public string Table { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is view.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is view; otherwise, <c>false</c>.
    /// </value>
    public bool IsView { get; set; }

    /// <summary>
    /// Gets the table name containing schema name dot table.
    /// </summary>
    /// <value>
    /// The table name.
    /// </value>
    public string Tablename => Schema.IsEmptyOrNull() ? Table : Schema + "." + Table;
}
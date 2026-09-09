namespace Serenity.Data.Schema;

/// <summary>
/// SQL metadata for a foreign key.
/// </summary>
public class ForeignKeyInfo
{
    /// <summary>
    /// Gets or sets the name of the foreign key.
    /// </summary>
    /// <value>
    /// The name of the foreign key.
    /// </value>
    public required string FKName { get; set; }

    /// <summary>
    /// Gets or sets the foreign key column.
    /// </summary>
    /// <value>
    /// The foreign key column.
    /// </value>
    public required string FKColumn { get; set; }

    /// <summary>
    /// Gets or sets the primary key schema.
    /// </summary>
    /// <value>
    /// The primary key schema.
    /// </value>
    public string? PKSchema { get; set; }

    /// <summary>
    /// Gets or sets the primary key table.
    /// </summary>
    /// <value>
    /// The primary key table.
    /// </value>
    public required string PKTable { get; set; }

    /// <summary>
    /// Gets or sets the primary key column.
    /// </summary>
    /// <value>
    /// The primary key column.
    /// </value>
    public required string PKColumn { get; set; }
}
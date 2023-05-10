namespace Serenity.Data.Schema;

/// <summary>
/// Meta data information for an SQL column
/// </summary>
public class FieldInfo
{
    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>
    /// The name of the field.
    /// </value>
    public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the size (max length or numeric precision).
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the numeric scale.
    /// </summary>
    /// <value>
    /// The numeric scale.
    /// </value>
    public int Scale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is primary key.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the field is primary key; otherwise, <c>false</c>.
    /// </value>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is an identity column.
    /// </summary>
    /// <value>
    ///   <c>true</c> if identity; otherwise, <c>false</c>.
    /// </value>
    public bool IsIdentity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is nullable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if nullable; otherwise, <c>false</c>.
    /// </value>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Gets or sets the primary key schema.
    /// </summary>
    /// <value>
    /// The primary key schema.
    /// </value>
    public string PKSchema { get; set; }

    /// <summary>
    /// Gets or sets the primary key table.
    /// </summary>
    /// <value>
    /// The primary key table.
    /// </value>
    public string PKTable { get; set; }

    /// <summary>
    /// Gets or sets the primary key column.
    /// </summary>
    /// <value>
    /// The primary key column.
    /// </value>
    public string PKColumn { get; set; }

    /// <summary>
    /// Gets or sets the type of the data.
    /// </summary>
    /// <value>
    /// The type of the data.
    /// </value>
    public string DataType { get; set; }
}
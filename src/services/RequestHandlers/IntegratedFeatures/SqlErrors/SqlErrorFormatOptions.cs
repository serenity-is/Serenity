namespace Serenity.Services.SqlErrors;

/// <summary>
/// Options that customize how a <see cref="SqlErrorInfo"/> is formatted as text.
/// </summary>
public class SqlErrorFormatOptions
{
    /// <summary>
    /// Gets or sets the operation that caused the error.
    /// </summary>
    public SqlErrorOperation? Operation { get; set; }

    /// <summary>
    /// Gets or sets the row type involved, if any.
    /// </summary>
    public Type? RowType { get; set; }

    /// <summary>
    /// Gets or sets the name of the table of the row involved, if known.
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Gets or sets the display name / title of the field involved, if known.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Gets or sets a custom local text key or literal error message. When it contains
    /// format placeholders, the table name and field name are passed as the arguments.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets a value indicating whether the operation is a delete or undelete.
    /// </summary>
    public bool IsDelete => Operation is SqlErrorOperation.Delete or SqlErrorOperation.Undelete;
}

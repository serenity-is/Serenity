namespace Serenity.Services;

/// <summary>
/// A validation context for rows
/// </summary>
/// <seealso cref="IValidationContext" />
public class RowValidationContext : IValidationContext
{
    private readonly IRow row;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowValidationContext"/> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="row">The row.</param>
    /// <param name="localizer">The localizer.</param>
    public RowValidationContext(IDbConnection connection, IRow row, ITextLocalizer localizer)
    {
        this.row = row;
        Connection = connection;
        Localizer = localizer;
    }

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    public object GetFieldValue(string fieldName)
    {
        var field = row.Fields.FindFieldByPropertyName(fieldName) ?? row.Fields.FindField(fieldName);
        if (field is null)
            return null;

        return field.AsObject(row);
    }

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    public IDbConnection Connection { get; private set; }
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value { get; set; }
    /// <summary>
    /// Localizer
    /// </summary>
    public ITextLocalizer Localizer { get; private set; }
}
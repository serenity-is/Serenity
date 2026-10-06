namespace Serenity.Services.SqlErrors;

/// <summary>
/// Information extracted from a database constraint violation.
/// </summary>
public class SqlErrorInfo
{
    /// <summary>
    /// Gets or sets the type of the constraint violation.
    /// </summary>
    public SqlErrorConstraintType Type { get; set; }

    /// <summary>
    /// Gets or sets the server type the exception came from, if it could be determined.
    /// </summary>
    public ServerType? ServerType { get; set; }

    /// <summary>
    /// Gets or sets the name of the table involved in the violation, if it could be determined.
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Gets or sets the name of the referenced (foreign) table for a foreign key violation,
    /// if it could be determined.
    /// </summary>
    public string? ReferencedTableName { get; set; }

    /// <summary>
    /// Gets or sets the names of the columns involved in the violation, if they could be determined.
    /// </summary>
    public IReadOnlyList<string>? ColumnNames { get; set; }

    /// <summary>
    /// Gets or sets the name of the constraint or index involved, if it could be determined.
    /// </summary>
    public string? ConstraintName { get; set; }

    /// <summary>
    /// Gets or sets the key value that caused the violation, if it could be determined.
    /// </summary>
    public string? KeyValue { get; set; }

    /// <summary>
    /// Gets or sets the provider error number, if available.
    /// </summary>
    public string? ErrorNumber { get; set; }

    /// <summary>
    /// Gets or sets the SQL state code, if available.
    /// </summary>
    public string? SqlState { get; set; }

    /// <summary>
    /// Gets or sets the original exception message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets the first column name involved in the violation, if any.
    /// </summary>
    public string? ColumnName => ColumnNames is { Count: > 0 } ? ColumnNames[0] : null;

    /// <summary>
    /// Formats this error info as a localized validation message.
    /// </summary>
    /// <param name="localizer">Text localizer.</param>
    /// <param name="options">Optional formatting options, e.g. the operation that caused the error.</param>
    /// <exception cref="ArgumentNullException"><paramref name="localizer"/> is <c>null</c>.</exception>
    public string ToString(ITextLocalizer localizer, SqlErrorFormatOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(localizer);
        options ??= new SqlErrorFormatOptions();

        if (!string.IsNullOrEmpty(options.ErrorMessage))
        {
            var custom = localizer.TryGet(options.ErrorMessage!) ?? options.ErrorMessage!;
            return string.Format(CultureInfo.CurrentCulture, custom,
                TableName ?? options.TableName ?? "",
                options.FieldName ?? ColumnName ?? "");
        }

        string text;
        object?[] args;

        switch (Type)
        {
            case SqlErrorConstraintType.ForeignKey when options.IsDelete:
                var deleteTable = TableName ?? options.TableName;
                if (!string.IsNullOrEmpty(deleteTable))
                {
                    text = SqlErrorValidationTexts.DeleteForeignKeyError.ToString(localizer);
                    args = [deleteTable];
                }
                else
                {
                    text = SqlErrorValidationTexts.DeleteForeignKeyErrorGeneric.ToString(localizer);
                    args = [];
                }
                break;

            case SqlErrorConstraintType.ForeignKey:
                var referencedTable = ReferencedTableName ?? TableName ?? options.TableName;
                if (!string.IsNullOrEmpty(referencedTable))
                {
                    text = SqlErrorValidationTexts.SaveForeignKeyError.ToString(localizer);
                    args = [referencedTable];
                }
                else
                {
                    text = SqlErrorValidationTexts.SaveForeignKeyErrorGeneric.ToString(localizer);
                    args = [];
                }
                break;

            case SqlErrorConstraintType.NotNull:
                text = DataValidationTexts.FieldIsRequired.ToString(localizer);
                args = [options.FieldName ?? ColumnName ?? "???"];
                break;

            default:
                var field = options.FieldName ?? ColumnName;
                if (!string.IsNullOrEmpty(field))
                {
                    text = SqlErrorValidationTexts.SavePrimaryKeyError.ToString(localizer);
                    args = [TableName ?? options.TableName ?? "???", field];
                }
                else
                {
                    text = SqlErrorValidationTexts.SavePrimaryKeyErrorGeneric.ToString(localizer);
                    args = [];
                }
                break;
        }

        return string.Format(CultureInfo.CurrentCulture, text, args);
    }
}

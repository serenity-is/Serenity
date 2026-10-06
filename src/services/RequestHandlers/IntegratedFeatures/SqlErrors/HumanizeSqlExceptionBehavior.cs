namespace Serenity.Services.SqlErrors;

/// <summary>
/// Implicit save/delete/undelete behavior that converts recognized database
/// constraint violations (primary key, unique, foreign key, not null) into
/// localized <see cref="ValidationError"/>s using <see cref="ISqlErrorExtractor"/>
/// and <see cref="SqlErrorInfo.ToString(ITextLocalizer, SqlErrorFormatOptions?)"/>
/// instead of letting the raw provider exception surface as a server error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="extractor">SQL error extractor.</param>
/// <exception cref="ArgumentNullException"><paramref name="extractor"/> is <c>null</c>.</exception>
public class HumanizeSqlExceptionBehavior(ISqlErrorExtractor extractor) : IImplicitBehavior,
    ISaveBehaviorSync, ISaveExceptionBehavior,
    IDeleteBehaviorSync, IDeleteExceptionBehavior,
    IUndeleteBehaviorSync, IUndeleteExceptionBehavior
{
    private readonly ISqlErrorExtractor extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));

    /// <inheritdoc/>
    public bool ActivateFor(IRow row) => true;

    /// <inheritdoc/>
    public void OnException(ISaveRequestHandler handler, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(handler);
        Humanize(handler.Connection, handler.Row, exception,
            handler.IsCreate ? SqlErrorOperation.Insert : SqlErrorOperation.Update,
            handler.Context?.Localizer);
    }

    /// <inheritdoc/>
    public void OnException(IDeleteRequestHandler handler, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(handler);
        Humanize(handler.Connection, handler.Row, exception, SqlErrorOperation.Delete,
            handler.Context?.Localizer);
    }

    /// <inheritdoc/>
    public void OnException(IUndeleteRequestHandler handler, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(handler);
        Humanize(handler.Connection, handler.Row, exception, SqlErrorOperation.Undelete,
            handler.Context?.Localizer);
    }

    private void Humanize(IDbConnection? connection, IRow row, Exception exception,
        SqlErrorOperation operation, ITextLocalizer? localizer)
    {
        var info = extractor.Extract(exception, new SqlErrorExtractOptions
        {
            ServerType = (connection as IHasDialect)?.Dialect?.ServerType
        });

        if (info is null)
            return;

        localizer ??= NullTextLocalizer.Instance;

        var fieldTitle = info.Type is SqlErrorConstraintType.PrimaryKey or SqlErrorConstraintType.Unique
            ? (row as IIdRow)?.GetIdField()?.GetTitle(localizer)
            : null;

        var message = info.ToString(localizer, new SqlErrorFormatOptions
        {
            Operation = operation,
            RowType = row?.GetType(),
            FieldName = info.ColumnName is null ? fieldTitle : null
        });

        throw new ValidationError(GetErrorCode(info, operation), info.ColumnName, message);
    }

    private static string GetErrorCode(SqlErrorInfo info, SqlErrorOperation operation) => info.Type switch
    {
        SqlErrorConstraintType.PrimaryKey or SqlErrorConstraintType.Unique => "UniqueViolation",
        SqlErrorConstraintType.ForeignKey when operation is SqlErrorOperation.Delete or SqlErrorOperation.Undelete
            => "RelatedRecordExist",
        SqlErrorConstraintType.ForeignKey => "ForeignKeyViolation",
        SqlErrorConstraintType.NotNull => "Required",
        _ => "SqlError"
    };
}

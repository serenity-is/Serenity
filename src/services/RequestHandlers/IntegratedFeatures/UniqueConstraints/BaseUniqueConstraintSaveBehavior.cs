namespace Serenity.Services;

/// <summary>
/// Base class for unique constraint save behaviors (e.g. <see cref="UniqueFieldSaveBehavior"/>
/// and <see cref="UniqueConstraintSaveBehavior"/>). Contains the shared query building and
/// validation logic, with virtual hooks so applications can customize how the pre-check query
/// is built, for example to apply row-level (tenant/owner) scoping so the pre-check matches
/// the actual database unique constraint.
/// </summary>
public abstract class BaseUniqueConstraintSaveBehavior : BaseSaveBehaviorAsync, ISaveBehaviorSync
{
    /// <summary>
    /// Validates that no record violates the unique constraint defined by the passed fields.
    /// </summary>
    /// <param name="handler">Save request handler.</param>
    /// <param name="fields">Fields that make up the unique constraint.</param>
    /// <param name="localizer">Text localizer.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <param name="groupCriteria">Optional additional criteria to scope the check.</param>
    /// <exception cref="ValidationError">A record with the same unique values already exists.</exception>
    protected virtual void ValidateUniqueConstraint(ISaveRequestHandler handler, IEnumerable<Field> fields,
        ITextLocalizer localizer, string? errorMessage = null, BaseCriteria? groupCriteria = null)
    {
        if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old!, handler.Row, StringComparer.Ordinal) != 0))
            return;

        var query = BuildUniqueConstraintQuery(handler, fields, groupCriteria);

        if (query.Exists(handler.UnitOfWork.Connection))
            throw UniqueViolation(fields, localizer, errorMessage);
    }

    /// <summary>
    /// Asynchronous version of <see cref="ValidateUniqueConstraint"/>.
    /// </summary>
    /// <param name="handler">Save request handler.</param>
    /// <param name="fields">Fields that make up the unique constraint.</param>
    /// <param name="localizer">Text localizer.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <param name="groupCriteria">Optional additional criteria to scope the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ValidationError">A record with the same unique values already exists.</exception>
    protected virtual async Task ValidateUniqueConstraintAsync(ISaveRequestHandler handler, IEnumerable<Field> fields,
        ITextLocalizer localizer, string? errorMessage = null, BaseCriteria? groupCriteria = null,
        CancellationToken cancellationToken = default)
    {
        if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old!, handler.Row, StringComparer.Ordinal) != 0))
            return;

        var query = BuildUniqueConstraintQuery(handler, fields, groupCriteria);

        if (await query.ExistsAsync(handler.UnitOfWork.Connection, cancellationToken: cancellationToken).ConfigureAwait(false))
            throw UniqueViolation(fields, localizer, errorMessage);
    }

    /// <summary>
    /// Builds the query used to check the unique constraint. Override this method to customize
    /// the pre-check, for example to add row-level scoping (tenant/owner filters) so that the
    /// check matches the actual database unique constraint, or to skip the pre-check entirely.
    /// </summary>
    /// <param name="handler">Save request handler.</param>
    /// <param name="fields">Fields that make up the unique constraint.</param>
    /// <param name="groupCriteria">Optional additional criteria to scope the check.</param>
    /// <returns>The query to run.</returns>
    protected virtual SqlQuery BuildUniqueConstraintQuery(ISaveRequestHandler handler,
        IEnumerable<Field> fields, BaseCriteria? groupCriteria)
    {
        var criteria = groupCriteria ?? Criteria.Empty;

        foreach (var field in fields)
        {
            if (field.IsNull(handler.Row))
                criteria &= field.IsNull();
            else
                criteria &= field == new ValueCriteria(field.AsSqlValue(handler.Row));
        }

        var idField = handler.Row.GetIdField();

        if (handler.IsUpdate)
            criteria &= idField != new ValueCriteria(idField.AsSqlValue(handler.Old!));

        var row = handler.Row.CreateNew();
        return new SqlQuery()
            .Dialect(handler.Connection.GetDialect())
            .From(row)
            .Select("1")
            .Where(criteria);
    }

    /// <summary>
    /// Creates the validation error that is thrown when a unique constraint is violated.
    /// </summary>
    /// <param name="fields">Fields that make up the unique constraint.</param>
    /// <param name="localizer">Text localizer.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    protected static ValidationError UniqueViolation(IEnumerable<Field> fields,
        ITextLocalizer localizer, string? errorMessage)
    {
        return new ValidationError("UniqueViolation",
            string.Join(", ", fields.Select(x => x.PropertyName ?? x.Name)),
            string.Format(!string.IsNullOrEmpty(errorMessage) ?
                (localizer.TryGet(errorMessage) ?? errorMessage) :
                    localizer.Get("Validation.UniqueConstraint"),
                string.Join(", ", fields.Select(x => x.GetTitle(localizer)))));
    }
}

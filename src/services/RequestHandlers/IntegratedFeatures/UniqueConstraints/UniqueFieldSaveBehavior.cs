namespace Serenity.Services;

/// <summary>
/// Interface that handles <see cref="UniqueConstraintAttribute"/> on fields.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="localizer">Text localizer</param>
public class UniqueFieldSaveBehavior(ITextLocalizer localizer) : BaseSaveBehaviorAsync, ISaveBehaviorSync, IImplicitBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private readonly ITextLocalizer localizer = localizer;

    private UniqueAttribute attr;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        if (!Target.Flags.HasFlag(FieldFlags.Unique))
            return false;

        var attr = Target.GetAttribute<UniqueAttribute>();
        if (attr != null && !attr.CheckBeforeSave)
            return false;

        this.attr = attr;
        return true;
    }

    /// <inheritdoc/>
    public virtual void OnBeforeSave(ISaveRequestHandler handler)
    {
        if (attr?.IgnoreNulls == true &&
            Target.IsNull(handler.Row))
            return;

        ValidateUniqueConstraint(handler, [Target], localizer,
            attr?.ErrorMessage,
            attr != null && attr.IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
    }

    /// <inheritdoc/>
    public override async Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (attr?.IgnoreNulls == true &&
            Target.IsNull(handler.Row))
            return;

        await ValidateUniqueConstraintAsync(handler, [Target], localizer,
            attr?.ErrorMessage,
            attr != null && attr.IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty,
            cancellationToken).ConfigureAwait(false);
    }

    internal static void ValidateUniqueConstraint(ISaveRequestHandler handler, IEnumerable<Field> fields, 
        ITextLocalizer localizer, string errorMessage = null, BaseCriteria groupCriteria = null)
    {
        if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old, handler.Row) != 0))
            return;

        var query = BuildUniqueConstraintQuery(handler, fields, groupCriteria);

        if (query.Exists(handler.UnitOfWork.Connection))
        {
            throw UniqueViolation(fields, localizer, errorMessage);
        }
    }

    internal static async Task ValidateUniqueConstraintAsync(ISaveRequestHandler handler, IEnumerable<Field> fields,
        ITextLocalizer localizer, string errorMessage = null, BaseCriteria groupCriteria = null,
        CancellationToken cancellationToken = default)
    {
        if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old, handler.Row) != 0))
            return;

        var query = BuildUniqueConstraintQuery(handler, fields, groupCriteria);

        if (await query.ExistsAsync(handler.UnitOfWork.Connection, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            throw UniqueViolation(fields, localizer, errorMessage);
        }
    }

    private static SqlQuery BuildUniqueConstraintQuery(ISaveRequestHandler handler,
        IEnumerable<Field> fields, BaseCriteria groupCriteria)
    {
        var criteria = groupCriteria ?? Criteria.Empty;

        foreach (var field in fields)
            if (field.IsNull(handler.Row))
                criteria &= field.IsNull();
            else
                criteria &= field == new ValueCriteria(field.AsSqlValue(handler.Row));

        var idField = ((IIdRow)handler.Row).IdField;

        if (handler.IsUpdate)
            criteria &= idField != new ValueCriteria(idField.AsSqlValue(handler.Old));

        var row = handler.Row.CreateNew();
        return new SqlQuery()
            .Dialect(handler.Connection.GetDialect())
            .From(row)
            .Select("1")
            .Where(criteria);
    }

    private static ValidationError UniqueViolation(IEnumerable<Field> fields,
        ITextLocalizer localizer, string errorMessage)
    {
        return new ValidationError("UniqueViolation",
            string.Join(", ", fields.Select(x => x.PropertyName ?? x.Name)),
            string.Format(!string.IsNullOrEmpty(errorMessage) ?
                (localizer.TryGet(errorMessage) ?? errorMessage) :
                    localizer.Get("Validation.UniqueConstraint"),
                string.Join(", ", fields.Select(x => x.GetTitle(localizer)))));
    }
}
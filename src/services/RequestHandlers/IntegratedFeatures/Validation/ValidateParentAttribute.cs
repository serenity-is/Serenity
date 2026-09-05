namespace Serenity.Services;

/// <summary>
/// Validation behavior for rows that have a parent record via <see cref="IParentIdRow"/> interface.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="rowTypeRegistry">Row type registry</param>
/// <param name="localizer">Text localizer</param>
/// <exception cref="ArgumentNullException"><paramref name="rowTypeRegistry"/> or <paramref name="localizer"/> is <c>null</c>.</exception>
public class ValidateParentBehavior(IRowTypeRegistry rowTypeRegistry, ITextLocalizer localizer) :
    BaseSaveBehaviorAsync, ISaveBehaviorSync
{
    private readonly IRowTypeRegistry rowTypeRegistry = rowTypeRegistry ??
            throw new ArgumentNullException(nameof(rowTypeRegistry));
    private readonly ITextLocalizer localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <inheritdoc/>
    public virtual void OnValidateRequest(ISaveRequestHandler handler)
    {
        if (!TryGetParentCheck(handler, out var tableName, out var criteria))
            return;

        ServiceHelper.CheckParentNotDeleted(handler.UnitOfWork.Connection,
            tableName, query => query.Where(criteria), localizer);
    }

    /// <inheritdoc/>
    public override async Task OnValidateRequestAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (!TryGetParentCheck(handler, out var tableName, out var criteria))
            return;

        await ServiceHelper.CheckParentNotDeletedAsync(handler.UnitOfWork.Connection,
            tableName, query => query.Where(criteria), localizer, cancellationToken).ConfigureAwait(false);
    }

    private bool TryGetParentCheck(ISaveRequestHandler handler, out string tableName, out BaseCriteria criteria)
    {
        tableName = null;
        criteria = null;

        var row = handler.Row;
        var old = handler.Old;
        var isUpdate = old != null;

        if (row is not IParentIdRow parentIdRow)
            return false;

        var parentIdField = parentIdRow.ParentIdField;
        var parentId = parentIdField.AsObject(row);
        if (parentId == null)
            return false;

        if (isUpdate && parentIdField.IndexCompare(old, row) == 0)
            return false;

        if (string.IsNullOrEmpty(parentIdField.ForeignTable))
            return false;

        var foreignRowType = rowTypeRegistry.ByConnectionKey(row.GetFields().ConnectionKey)
            .FirstOrDefault(x => x.GetCustomAttribute<TableNameAttribute>()?.Name ==
                parentIdField.ForeignTable);

        if (foreignRowType == null)
            return false;

        if (Activator.CreateInstance(foreignRowType) is not IIdRow foreignRow ||
            foreignRow is not IIsActiveRow iar)
            return false;

        tableName = foreignRow.Table;
        criteria = new Criteria(foreignRow.IdField) == new ValueCriteria(parentId) &
            new Criteria(iar.IsActiveField) < 0;

        return true;
    }
}

namespace Serenity.Services;

/// <summary>
/// Validation behavior for rows that have a parent record via <see cref="IParentIdRow"/> interface
/// </summary>
public class ValidateParentBehavior : BaseSaveBehavior
{
    private readonly IRowTypeRegistry rowTypeRegistry;
    private readonly ITextLocalizer localizer;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="rowTypeRegistry">Row type registry</param>
    /// <param name="localizer">Text localizer</param>
    /// <exception cref="ArgumentNullException">rowTypeRegistry or localizer is null</exception>
    public ValidateParentBehavior(IRowTypeRegistry rowTypeRegistry, ITextLocalizer localizer)
    {
        this.rowTypeRegistry = rowTypeRegistry ?? 
            throw new ArgumentNullException(nameof(rowTypeRegistry));
        this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    /// <inheritdoc/>
    public override void OnValidateRequest(ISaveRequestHandler handler)
    {
        base.OnValidateRequest(handler);

        var row = handler.Row;
        var old = handler.Old;
        var isUpdate = old == null;

        if (row is not IParentIdRow parentIdRow)
            return;

        var parentId = parentIdRow.ParentIdField.AsObject(row);
        if (parentId == null)
            return;

        if (isUpdate && parentId == parentIdRow.ParentIdField.AsObject(old))
            return;

        var parentIdField = parentIdRow.ParentIdField;
        if (parentIdField.ForeignTable.IsNullOrEmpty())
            return;

        var foreignRowType = rowTypeRegistry.ByConnectionKey(row.GetFields().ConnectionKey)
            .FirstOrDefault(x => x.GetCustomAttribute<TableNameAttribute>()?.Name == 
                parentIdField.ForeignTable);

        if (foreignRowType == null)
            return;

        if (Activator.CreateInstance(foreignRowType) is not IIdRow foreignRow ||
            foreignRow is not IIsActiveRow iar)
            return;

        ServiceHelper.CheckParentNotDeleted(handler.UnitOfWork.Connection, 
            foreignRow.Table,
            query => query.Where(
                new Criteria(foreignRow.IdField) == new ValueCriteria(parentId) &
                new Criteria(iar.IsActiveField) < 0), localizer);
    }
}
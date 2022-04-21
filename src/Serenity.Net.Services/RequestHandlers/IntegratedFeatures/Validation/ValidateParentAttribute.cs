namespace Serenity.Services
{
    public class ValidateParentBehavior : BaseSaveBehavior
    {
        private readonly IRowTypeRegistry rowTypeRegistry;
        private readonly ITextLocalizer localizer;

        public ValidateParentBehavior(IRowTypeRegistry rowTypeRegistry, ITextLocalizer localizer)
        {
            this.rowTypeRegistry = rowTypeRegistry ?? 
                throw new ArgumentNullException(nameof(rowTypeRegistry));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public override void OnValidateRequest(ISaveRequestHandler handler)
        {
            base.OnValidateRequest(handler);

            var row = handler.Row;
            var old = handler.Old;
            var isUpdate = old == null;

            if (!(row is IParentIdRow parentIdRow))
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

            if (!(Activator.CreateInstance(foreignRowType) is IIdRow foreignRow) ||
                !(foreignRow is IIsActiveRow iar))
                return;

            ServiceHelper.CheckParentNotDeleted(handler.UnitOfWork.Connection, 
                foreignRow.Table,
                query => query.Where(
                    new Criteria(foreignRow.IdField) == new ValueCriteria(parentId) &
                    new Criteria(iar.IsActiveField) < 0), localizer);
        }
    }
}
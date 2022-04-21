namespace Serenity.Services
{
    public class UniqueFieldSaveBehavior : BaseSaveBehavior, IImplicitBehavior, IFieldBehavior
    {
        public Field Target { get; set; }
        public ITextLocalizer Localizer { get; }

        private UniqueAttribute attr;

        public UniqueFieldSaveBehavior(ITextLocalizer localizer)
        {
            Localizer = localizer;
        }

        public bool ActivateFor(IRow row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            if (!Target.Flags.HasFlag(FieldFlags.Unique))
                return false;

            var attr = Target.GetAttribute<UniqueAttribute>();
            if (attr != null && !attr.CheckBeforeSave)
                return false;

            this.attr = attr;
            return true;
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            ValidateUniqueConstraint(handler, new Field[] { Target }, Localizer,
                attr == null ? (string)null : attr.ErrorMessage,
                attr != null && attr.IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
        }

        internal static void ValidateUniqueConstraint(ISaveRequestHandler handler, IEnumerable<Field> fields, 
            ITextLocalizer localizer, string errorMessage = null, BaseCriteria groupCriteria = null)
        {
            if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old, handler.Row) != 0))
                return;

            var criteria = groupCriteria ?? Criteria.Empty;

            foreach (var field in fields)
                if (field.IsNull(handler.Row))
                    criteria &= field.IsNull();
                else
                    criteria &= field == new ValueCriteria(field.AsSqlValue(handler.Row));

            var idField = (Field)((IIdRow)handler.Row).IdField;

            if (handler.IsUpdate)
                criteria &= (Field)idField != new ValueCriteria(idField.AsSqlValue(handler.Old));

            var row = handler.Row.CreateNew();
            if (new SqlQuery()
                    .Dialect(handler.Connection.GetDialect())
                    .From(row)
                    .Select("1")
                    .Where(criteria)
                    .Exists(handler.UnitOfWork.Connection))
            { 
                throw new ValidationError("UniqueViolation",
                    String.Join(", ", fields.Select(x => x.PropertyName ?? x.Name)),
                    string.Format(!string.IsNullOrEmpty(errorMessage) ?
                        (localizer.TryGet(errorMessage) ?? errorMessage) :
                            localizer.Get("Validation.UniqueConstraint"),
                        String.Join(", ", fields.Select(x => x.GetTitle(localizer)))));
            }
        }
    }
}
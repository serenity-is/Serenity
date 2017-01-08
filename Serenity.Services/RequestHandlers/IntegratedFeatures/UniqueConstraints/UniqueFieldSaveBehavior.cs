using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public class UniqueFieldSaveBehavior : BaseSaveBehavior, IImplicitBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private UniqueAttribute attr;

        public bool ActivateFor(Row row)
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

        public override void OnValidateRequest(ISaveRequestHandler handler)
        {
            ValidateUniqueConstraint(handler, new Field[] { Target },
                attr == null ? (string)null : attr.ErrorMessage,
                Criteria.Empty);
        }

        internal static void ValidateUniqueConstraint(ISaveRequestHandler handler, IEnumerable<Field> fields,
            string errorMessage = null, BaseCriteria groupCriteria = null)
        {
            if (handler.IsUpdate && !fields.Any(x => x.IndexCompare(handler.Old, handler.Row) != 0))
                return;

            var criteria = groupCriteria ?? Criteria.Empty;

            foreach (var field in fields)
                if (field.IsNull(handler.Row))
                    criteria &= field.IsNull();
                else
                    criteria &= field == new ValueCriteria(field.AsObject(handler.Row));

            var idField = (Field)((IIdRow)handler.Row).IdField;

            if (handler.IsUpdate)
                criteria &= (Field)idField != new ValueCriteria(idField.AsObject(handler.Old));

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
                    String.Format(!string.IsNullOrEmpty(errorMessage) ?
                        (LocalText.TryGet(errorMessage) ?? errorMessage) :
                            LocalText.Get("Validation.UniqueConstraint"),
                        String.Join(", ", fields.Select(x => x.Title))));
            }
        }
    }
}
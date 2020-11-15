using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class UniqueConstraintSaveBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        private UniqueConstraintAttribute[] attrList;
        private IEnumerable<Field>[] attrFields;

        public bool ActivateFor(Row row)
        {
            var attr = row.GetType().GetCustomAttributes<UniqueConstraintAttribute>()
                .Where(x => x.CheckBeforeSave);

            if (!attr.Any())
                return false;

            this.attrList = attr.ToArray();
            return true;
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            if (attrList == null)
                return;

            if (attrFields == null)
            {
                attrFields = attrList.Select(attr =>
                {
                    return attr.Fields.Select(x =>
                    {
                        var field = handler.Row.FindFieldByPropertyName(x) ?? handler.Row.FindField(x);
                        if (ReferenceEquals(null, field))
                        {
                            throw new InvalidOperationException(String.Format(
                                "Can't find field '{0}' of unique constraint in row type '{1}'",
                                    x, handler.Row.GetType().FullName));
                        }
                        return field;
                    });
                }).ToArray();
            }

            for (var i = 0; i < attrList.Length; i++)
            {
                var attr = attrList[i];
                var fields = attrFields[i];

                UniqueFieldSaveBehavior.ValidateUniqueConstraint(handler, fields, attr.ErrorMessage, 
                    attrList[i].IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
            }
        }
    }
}
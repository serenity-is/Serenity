using Serenity;
using Serenity.Data;
using Serenity.Data.Mapping;
using Serenity.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    public class UpdateInsertLogBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        public bool ActivateFor(Row row)
        {
            return row is IUpdateLogRow || row is IInsertLogRow;
        }

        public override void OnSetInternalFields(ISaveRequestHandler handler)
        {
            var row = handler.Row;
            var updateLogRow = row as IUpdateLogRow;
            var insertLogRow = row as IInsertLogRow;

            if (updateLogRow != null && (handler.IsUpdate || insertLogRow == null))
            {
                updateLogRow.UpdateDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now, updateLogRow.UpdateDateField.DateTimeKind);
                updateLogRow.UpdateUserIdField[row] = Authorization.UserId.TryParseID();
            }
            else if (insertLogRow != null && handler.IsCreate)
            {
                insertLogRow.InsertDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now, insertLogRow.InsertDateField.DateTimeKind);
                insertLogRow.InsertUserIdField[row] = Authorization.UserId.TryParseID();
            }
        }
    }
}
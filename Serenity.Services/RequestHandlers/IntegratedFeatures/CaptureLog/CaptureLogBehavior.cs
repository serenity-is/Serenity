using Serenity.Data;
using System;
using System.Reflection;

namespace Serenity.Services
{
    public class CaptureLogBehavior : BaseSaveDeleteBehavior, IImplicitBehavior
    {
        private ICaptureLogHandler captureLogHandler;

        public bool ActivateFor(Row row)
        {
            if (row.GetType().GetCustomAttribute<CaptureLogAttribute>() == null)
                return false;

            if (!(row is IIdRow))
                return false;

            captureLogHandler = (ICaptureLogHandler)Activator.CreateInstance(
                typeof(CaptureLogHandler<>).MakeGenericType(row.GetType()));

            return true;
        }

        public override void OnAudit(IDeleteRequestHandler handler)
        {
            if (handler.Row == null || captureLogHandler == null)
                return;

            Row newRow = null;

            // if row is not actually deleted, but set to deleted by a flag, log it as if it is an update operation
            if (handler.Row is IIsActiveDeletedRow)
            {
                newRow = handler.Row.Clone();
                ((IIsActiveDeletedRow)newRow).IsActiveField[newRow] = -1;
            }

            captureLogHandler.Log(handler.UnitOfWork, handler.Row, newRow, Authorization.UserId);
        }

        public override void OnAudit(ISaveRequestHandler handler)
        {
            if (handler.Row == null || captureLogHandler == null)
                return;

            if (handler.IsCreate)
            {
                captureLogHandler.Log(handler.UnitOfWork, 
                    null, handler.Row, Authorization.UserId);

                return;
            }

            var insertLogRow = handler.Row as IInsertLogRow;
            var updateLogRow = handler.Row as IUpdateLogRow;

            bool anyChanged = false;
            foreach (var field in handler.Row.GetTableFields())
            {
                if (insertLogRow != null &&
                    (ReferenceEquals(insertLogRow.InsertDateField, field) ||
                     ReferenceEquals(insertLogRow.InsertUserIdField, field)))
                    continue;

                if (updateLogRow != null && 
                    (ReferenceEquals(updateLogRow.UpdateDateField, field) ||
                     ReferenceEquals(updateLogRow.UpdateUserIdField, field)))
                {
                    continue;
                }

                if (field.IndexCompare(handler.Old, handler.Row) != 0)
                {
                    anyChanged = true;
                    break;
                }
            }

            if (anyChanged)
                captureLogHandler.Log(handler.UnitOfWork, 
                    handler.Old, handler.Row, Authorization.UserId);
        }
    }
}
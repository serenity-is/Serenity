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

            captureLogHandler = (ICaptureLogHandler)Activator.CreateInstance(
                typeof(CaptureLogHandler<>).MakeGenericType(row.GetType()));

            return true;
        }

        public override void OnAudit(IDeleteRequestHandler handler)
        {
            if (handler.Row == null || captureLogHandler == null)
                return;

            captureLogHandler.LogDelete(handler.UnitOfWork, handler.Row, Authorization.UserId.TryParseID().Value);
        }

        public override void OnAudit(ISaveRequestHandler handler)
        {
            if (handler.Row == null || captureLogHandler == null)
                return;

            if (handler.IsCreate)
            {
                captureLogHandler.LogSave(handler.UnitOfWork, handler.Row, 
                    Authorization.UserId.TryParseID().Value);

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
                captureLogHandler.LogSave(handler.UnitOfWork, handler.Row, Authorization.UserId.TryParseID().Value);
        }
    }
}
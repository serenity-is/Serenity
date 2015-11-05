using Serenity.Data;
using System;
using System.Reflection;

namespace Serenity.Services
{
    public class CaptureLogSaveBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        private ICaptureLogHandler captureLogHandler;

        public bool ActivateFor(Row row)
        {
            return row.GetType().GetCustomAttribute<CaptureLogAttribute>() != null;
        }

        public override void OnAudit(ISaveRequestHandler handler)
        {
            if (captureLogHandler == null)
            {
                captureLogHandler = (ICaptureLogHandler)Activator.CreateInstance(
                    typeof(CaptureLogHandler<>).MakeGenericType(handler.Row.GetType()));
            }

            if (handler.Row == null)
                return;

            if (handler.Old == null)
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
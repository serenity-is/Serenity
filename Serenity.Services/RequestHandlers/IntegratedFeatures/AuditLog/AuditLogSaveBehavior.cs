using Serenity.Data;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    public class AuditLogSaveBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        private string connectionKey;

        public bool ActivateFor(Row row)
        {
            return row.GetType().GetCustomAttribute<AuditLogAttribute>() != null;
        }

        protected AuditSaveRequest GetAuditRequest(ISaveRequestHandler handler)
        {
            bool isCreate = handler.Old == null;

            var auditFields = new HashSet<Field>();
            var flag = isCreate ? FieldFlags.Insertable : FieldFlags.Updatable;
            foreach (var field in handler.Row.GetFields())
                if (field.Flags.HasFlag(flag))
                    auditFields.Add(field);

            Field[] array = new Field[auditFields.Count];
            auditFields.CopyTo(array);

            var auditRequest = new AuditSaveRequest(handler.Row.Table, (IIdRow)handler.Old, (IIdRow)handler.Row, array);

            var parentIdRow = handler.Row as IParentIdRow;
            if (parentIdRow != null)
            {
                var parentIdField = (Field)parentIdRow.ParentIdField;

                if (!parentIdField.ForeignTable.IsTrimmedEmpty())
                {
                    auditRequest.ParentTypeId = parentIdField.ForeignTable;
                    auditRequest.OldParentId = handler.Old == null ? null : parentIdRow.ParentIdField[handler.Old];
                    auditRequest.NewParentId = parentIdRow.ParentIdField[handler.Row];
                }
            }

            return auditRequest;
        }


        public override void OnAudit(ISaveRequestHandler handler)
        {
            if (handler.Row == null)
                return;

            var auditRequest = GetAuditRequest(handler);

            if (connectionKey == null)
                connectionKey = RowRegistry.GetConnectionKey(handler.Row);

            if (handler.Old == null)
            {
                if (auditRequest != null)
                    AuditLogService.AuditInsert(handler.UnitOfWork.Connection, connectionKey, auditRequest);

                return;
            }

            Row audit = null;
            if (auditRequest != null)
                audit = AuditLogService.PrepareAuditUpdate(connectionKey, auditRequest);

            if (audit != null)
                handler.UnitOfWork.Connection.Insert(audit);
        }
    }
}
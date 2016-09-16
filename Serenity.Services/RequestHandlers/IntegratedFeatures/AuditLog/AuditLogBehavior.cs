using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    public class AuditLogBehavior : BaseSaveDeleteBehavior, IImplicitBehavior
    {
        private string connectionKey;

        public bool ActivateFor(Row row)
        {
            return row.GetType().GetCustomAttribute<AuditLogAttribute>() != null &&
                row is IIdRow &&
                ((IIdRow)row).IdField.IsIntegerType;
        }

        protected AuditDeleteRequest GetAuditDeleteRequest(IDeleteRequestHandler handler)
        {
            var idField = ((IIdRow)handler.Row).IdField;

            var auditRequest = new AuditDeleteRequest(handler.Row.Table, idField[handler.Row].Value);

            var parentIdRow = handler.Row as IParentIdRow;
            if (parentIdRow != null)
            {
                var parentIdField = (Field)parentIdRow.ParentIdField;
                //EntityType parentEntityType;
                if (!parentIdField.ForeignTable.IsNullOrEmpty())
                    //SiteSchema.Instance.TableToType.TryGetValue(parentIdField.ForeignTable, out parentEntityType))
                {
                    auditRequest.ParentTypeId = parentIdField.ForeignTable;
                    auditRequest.ParentId = parentIdRow.ParentIdField[handler.Row];
                }
            }

            return auditRequest;
        }

        public override void OnAudit(IDeleteRequestHandler handler)
        {
            var auditRequest = GetAuditDeleteRequest(handler);
            if (auditRequest != null)
                AuditLogService.AuditDelete(handler.UnitOfWork.Connection, RowRegistry.GetConnectionKey(handler.Row), auditRequest);
        }

        protected AuditSaveRequest GetAuditSaveRequest(ISaveRequestHandler handler)
        {
            var auditFields = new HashSet<Field>();
            var flag = handler.IsCreate ? FieldFlags.Insertable : FieldFlags.Updatable;
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
                    auditRequest.OldParentId = handler.IsCreate ? null : parentIdRow.ParentIdField[handler.Old];
                    auditRequest.NewParentId = parentIdRow.ParentIdField[handler.Row];
                }
            }

            return auditRequest;
        }

        public override void OnAudit(ISaveRequestHandler handler)
        {
            if (handler.Row == null)
                return;

            var auditRequest = GetAuditSaveRequest(handler);

            if (connectionKey == null)
                connectionKey = RowRegistry.GetConnectionKey(handler.Row);

            if (handler.IsCreate)
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
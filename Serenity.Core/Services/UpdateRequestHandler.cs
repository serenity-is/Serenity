using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Serenity.Services
{
    public class UpdateRequestHandler<TRow, TUpdateResponse>
        where TRow : Row, IIdRow, new()
        where TUpdateResponse : UpdateResponse, new()
    {
        protected TRow Row;
        protected TRow Old;
        protected IUnitOfWork UnitOfWork;
        protected TUpdateResponse Response;
        protected SaveRequest<TRow> Request;
        private static bool loggingInitialized;
        protected static CaptureLogHandler<TRow> captureLogHandler;
        protected static bool hasAuditLogAttribute;

        protected IDbConnection Connection
        {
            get { return UnitOfWork.Connection; }
        }
        
        protected virtual void GetEditableFields(HashSet<Field> editable)
        {
            foreach (var field in Row.GetFields())
                if ((field.Flags & FieldFlags.Updatable) == FieldFlags.Updatable)
                    editable.Add(field);
        }

        protected virtual void GetRequiredFields(HashSet<Field> required)
        {
            foreach (var field in Row.GetFields())
                if ((field.Flags & FieldFlags.Updatable) == FieldFlags.Updatable &
                    (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                    (field.Flags & FieldFlags.TrimToEmpty) != FieldFlags.TrimToEmpty)
                {
                    required.Add(field);
                }
        }

        protected virtual void SetInternalFields()
        {
            var loggingRow = Row as ILoggingRow;
            if (loggingRow != null)
            {
                loggingRow.UpdateDateField[Row] = DateTime.UtcNow;
                loggingRow.UpdateUserIdField[Row] = SecurityHelper.CurrentUserId;
            }
        }

        protected virtual void OnBeforeUpdate()
        {
        }

        protected virtual void OnAfterUpdate()
        {
        }

        protected virtual void OnReturn()
        {
        }

        protected virtual AuditUpdateRequest GetAuditRequest(HashSet<Field> auditFields)
        {
            Field[] array = new Field[auditFields.Count];
            auditFields.CopyTo(array);
            var auditRequest = new AuditUpdateRequest(Row.Table, (IIdRow)Old, (IIdRow)Row, array);

            var parentIdRow = Row as IParentIdRow;
            if (parentIdRow != null)
            {
                var parentIdField = (Field)parentIdRow.ParentIdField;
                //EntityType parentEntityType;
                if (!parentIdField.ForeignTable.IsTrimmedEmpty())
                    //SiteSchema.Instance.TableToType.TryGetValue(parentIdField.ForeignTable, out parentEntityType))
                {
                    auditRequest.ParentTypeId = parentIdField.ForeignTable;
                    auditRequest.OldParentId = parentIdRow.ParentIdField[Row];
                    auditRequest.NewParentId = parentIdRow.ParentIdField[Old];
                }
            }

            return auditRequest;
        }

        protected virtual SqlQuery PrepareQuery()
        {
            var idField = (Field)(Row.IdField);
            var id = Row.IdField[Row].Value;

            return new SqlQuery()
                .From(Old)
                .SelectTableFields()
                .WhereEqual(idField, id);
        }

        protected virtual void LoadOldEntity()
        {
            //if (Request.OldEntity != null)
            //{
            //    var row = Request.OldEntity;
            //    foreach (var field in row.GetFields())
            //        if (DbFieldExtensions.IsTableField(field))
            //        {
            //            if (row.IsAssigned(field))
            //                field.Copy(row, Old);
            //            else
            //                field.CopyNoAssignment(row, Old);
            //        }
            //    return;
            //}

            var idField = (Field)(Row.IdField);
            var id = Row.IdField[Row].Value;

            if (!PrepareQuery().GetFirst(Connection))
                throw DataValidation.EntityNotFoundError(Row, id);
        }

        protected virtual void HandleNonEditable(Field field)
        {
            if (field.IndexCompare(Row, Old) == 0)
            {
                field.CopyNoAssignment(Old, Row);
                Row.ClearAssignment(field);
                return;
            }

            if ((field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
            {
                bool isNonTableField = ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign) ||
                      ((field.Flags & FieldFlags.Calculated) == FieldFlags.Calculated) ||
                      ((field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide);

                if (!isNonTableField)
                    throw DataValidation.ReadOnlyError(Row, field);

                field.CopyNoAssignment(Old, Row);
                Row.ClearAssignment(field);
            }

            if (Row.IsAssigned(field))
                Row.ClearAssignment(field);
        }

        protected virtual void ValidateEditableFields(HashSet<Field> editable)
        {
            foreach (Field field in Row.GetFields())
            {
                if (!Row.IsAssigned(field))
                {
                    field.CopyNoAssignment(Old, Row);
                    Row.ClearAssignment(field);
                    continue;
                }

                var stringField = field as StringField;
                if (stringField != null &&
                    Row.IsAssigned(field) &&
                    (field.Flags & FieldFlags.Trim) == FieldFlags.Trim)
                {
                    string value = stringField[Row];

                    if ((field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                        value = value.TrimToEmpty();
                    else // TrimToNull
                        value = value.TrimToNull();

                    stringField[Row] = value;
                }

                if (!editable.Contains(field))
                    HandleNonEditable(field);
            }
        }

        protected virtual void ValidateEditable()
        {
            var editableFields = new HashSet<Field>();
            GetEditableFields(editableFields);
            ValidateEditableFields(editableFields);
        }

        protected virtual void ValidateRequired()
        {
            var requiredFields = new HashSet<Field>();
            GetRequiredFields(requiredFields);
            Row.ValidateRequiredIfModified(requiredFields);
        }

        protected virtual void ValidateRequest()
        {
            ValidateEditable();
            ValidateRequired();
            ValidateIsActive();
            ValidateParent();
        }

        protected virtual void ValidateIsActive()
        {
            var isActiveRow = Old as IIsActiveRow;
            if (isActiveRow != null &&
                isActiveRow.IsActiveField[Old] < 0)
                throw DataValidation.RecordNotActive(Old);
        }

        protected virtual void ValidateParent()
        {
            var parentIdRow = Row as IParentIdRow;
            if (parentIdRow == null)
                return;

            var parentId = parentIdRow.ParentIdField[Row];
            if (parentId == null)
                return;

            if (parentId == parentIdRow.ParentIdField[Old])
                return;

            var parentIdField = (Field)parentIdRow.ParentIdField;
            if (parentIdField.ForeignTable.IsEmptyOrNull())
                return;

            var foreignRow = RowRegistry.GetSchemaRow(RowRegistry.GetSchemaName(Row), parentIdField.ForeignTable);
            if (foreignRow == null)
                return;

            var idForeign = (IIdRow)foreignRow;
            if (idForeign == null)
                return;

            var isActiveForeign = (IIsActiveRow)foreignRow;
            if (isActiveForeign == null)
                return;

            ServiceHelper.CheckParentNotDeleted(Connection, foreignRow.Table, query => query
                .Where(
                    new Criteria((Field)idForeign.IdField) == parentId.Value &
                    new Criteria(isActiveForeign.IsActiveField) < 0));
        }

        protected virtual void DoGenericAudit()
        {
            var auditFields = new HashSet<Field>();
            GetEditableFields(auditFields);

            var auditRequest = GetAuditRequest(auditFields);

            Row audit = null;
            if (auditRequest != null)
                audit = AuditLogService.PrepareAuditUpdate(RowRegistry.GetSchemaName(Row), auditRequest);

            if (audit != null)
                new SqlInsert(audit).Execute(Connection);
        }

        protected virtual void DoCaptureLog()
        {
            var logRow = Row as ILoggingRow;
            bool anyChanged = false;
            foreach (var field in this.Row.GetTableFields())
            {
                if (logRow != null &&
                    (logRow.InsertDateField == field ||
                     logRow.UpdateDateField == field ||
                     logRow.InsertUserIdField == field ||
                     logRow.UpdateUserIdField == field))
                {
                    continue;
                }

                if (field.IndexCompare(Old, Row) != 0)
                {
                    anyChanged = true;
                    break;
                }
            }

            if (anyChanged)
                captureLogHandler.Log(this.UnitOfWork, this.Row, SecurityHelper.CurrentUserId, isDelete: false);
        }

        protected virtual void DoAudit()
        {
            if (!loggingInitialized)
            {
                var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>();
                if (logTableAttr != null)
                    captureLogHandler = new CaptureLogHandler<TRow>();

                hasAuditLogAttribute = typeof(TRow).GetCustomAttribute<AuditLogAttribute>(false) != null;

                loggingInitialized = true;
            }

            if (captureLogHandler != null)
                DoCaptureLog();
            else if (hasAuditLogAttribute)
                DoGenericAudit();
        }

        protected virtual void ValidateAndClearIdField()
        {
            var idField = (Field)(Row.IdField);
            Row.ValidateRequired(idField);
            Row.ClearAssignment(idField);
        }

        protected virtual void ExecuteUpdate()
        {
            if (Row.IsAnyFieldAssigned)
            {
                var idField = (Field)(Row.IdField);
                var id = Row.IdField[Row].Value;
                if (new SqlUpdate(Row)
                    .WhereEqual(idField, id)
                    .Execute(Connection) != 1)
                    throw DataValidation.UnexpectedError();

                InvalidateCacheOnCommit();
            }
        }

        protected virtual void ClearNonTableAssignments()
        {
            foreach (var field in Row.GetFields())
                if (Row.IsAssigned(field) &&
                    (field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign ||
                    (field.Flags & FieldFlags.Calculated) == FieldFlags.Calculated ||
                    (field.Flags & FieldFlags.Reflective) == FieldFlags.Reflective ||
                    (field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide)
                {
                    Row.ClearAssignment(field);
                }
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (OperationPermissionAttribute)typeof(TRow).GetCustomAttribute<UpdatePermissionAttribute>(false) ??
                typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false);

            if (attr != null)
            {
                if (attr.Permission.IsEmptyOrNull())
                    SecurityHelper.EnsureLoggedIn(RightErrorHandling.ThrowException);
                else
                    SecurityHelper.EnsurePermission(attr.Permission, RightErrorHandling.ThrowException);
            }
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            var attr = typeof(TRow).GetCustomAttribute<TwoLevelCachedAttribute>(false);
            if (attr != null)
            {
                BatchGenerationUpdater.OnCommit(this.UnitOfWork, Row.GetFields().GenerationKey);
                foreach (var key in attr.GenerationKeys)
                    BatchGenerationUpdater.OnCommit(this.UnitOfWork, key);
            }
        }

        public TUpdateResponse Process(IUnitOfWork unitOfWork, SaveRequest<TRow> request)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            UnitOfWork = unitOfWork;

            Request = request;
            Response = new TUpdateResponse();

            Row = request.Entity;
            if (Row == null)
                throw new ArgumentNullException("Entity");

            ValidateAndClearIdField();
            Old = new TRow();
            LoadOldEntity();

            ValidatePermissions();
            ValidateRequest();

            SetInternalFields();
            OnBeforeUpdate();
            ClearNonTableAssignments();
            ExecuteUpdate();
            OnAfterUpdate();
            DoAudit();
            OnReturn();

            return Response;
        }
    }

    public class UpdateRequestHandler<TRow> : UpdateRequestHandler<TRow, UpdateResponse>
        where TRow : Row, IIdRow, new()
    {
    }
}
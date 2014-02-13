using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using EntityType = System.String;

namespace Serenity.Services
{
    public class CreateRequestHandler<TRow, TCreateRequest, TCreateResponse>
        where TRow : Row, IIdRow, new()
        where TCreateRequest: SaveRequest<TRow>
        where TCreateResponse : CreateResponse, new()
    {
        protected TRow Row;
        protected IUnitOfWork UnitOfWork;
        protected TCreateRequest Request;
        protected TCreateResponse Response;

        private static bool loggingInitialized;
        protected static CaptureLogHandler<TRow> captureLogHandler;
        protected static bool hasAuditLogAttribute;

        private bool _displayOrderFix;

        protected IDbConnection Connection 
        { 
            get { return UnitOfWork.Connection; } 
        }

        protected virtual void GetEditableFields(HashSet<Field> editable)
        {
            foreach (var field in Row.GetFields())
                if ((field.Flags & FieldFlags.Insertable) == FieldFlags.Insertable)
                    editable.Add(field);
        }

        protected virtual void GetRequiredFields(HashSet<Field> required, HashSet<Field> editable)
        {
            foreach (var field in Row.GetFields())
                if (editable.Contains(field) &&
                    (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                    (field.Flags & FieldFlags.TrimToEmpty) != FieldFlags.TrimToEmpty)
                    required.Add(field);
        }

        protected virtual void SetInternalFields()
        {
            SetDisplayOrderField();

            var loggingRow = Row as ILoggingRow;
            if (loggingRow != null)
            {
                loggingRow.InsertDateField[Row] = DateTimeField.ToDateTimeKind(DateTime.Now, loggingRow.InsertDateField.DateTimeKind);
                loggingRow.InsertUserIdField[Row] = SecurityHelper.CurrentUserId;
            }

            var isActiveRow = Row as IIsActiveRow;
            if (isActiveRow != null)
                isActiveRow.IsActiveField[Row] = 1;

            foreach (var field in Row.GetFields())
                if (!Row.IsAssigned(field) &&
                    (field is StringField &&
                    (field.Flags & FieldFlags.Insertable) == FieldFlags.Insertable &
                    (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                    (field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty))
                {
                    ((StringField)field)[Row] = "";
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

        protected virtual void BeforeInsert()
        {
        }

        protected virtual void AfterInsert()
        {
            if (_displayOrderFix)
            {
                var displayOrderRow = Row as IDisplayOrderRow;
                DisplayOrderHelper.ReorderValues(Connection, displayOrderRow,
                    GetDisplayOrderFilter(), Response.EntityId, displayOrderRow.DisplayOrderField[Row].Value, false);
            }
        }

        protected virtual void OnReturn()
        {
        }

        protected AuditInsertRequest GetAuditRequest(HashSet<Field> auditFields)
        {
            EntityType entityType = Row.Table;

            Field[] array = new Field[auditFields.Count];
            auditFields.CopyTo(array);
            var auditRequest = new AuditInsertRequest(entityType, Row, array);

            var parentIdRow = Row as IParentIdRow;
            if (parentIdRow != null)
            {
                var parentIdField = (Field)parentIdRow.ParentIdField;
                if (!parentIdField.ForeignTable.IsTrimmedEmpty())
                {
                    auditRequest.ParentTypeId = parentIdField.ForeignTable;
                    auditRequest.ParentId = parentIdRow.ParentIdField[Row];
                }
            }

            return auditRequest;
        }

        protected virtual void HandleNonEditable(Field field)
        {
            if (!field.IsNull(Row) &&
                (field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
            {
                bool isNonTableField = ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign) ||
                      ((field.Flags & FieldFlags.Calculated) == FieldFlags.Calculated) ||
                      ((field.Flags & FieldFlags.ClientSide) == FieldFlags.ClientSide);

                if (!isNonTableField)
                    throw DataValidation.ReadOnlyError(Row, field);

                field.AsObject(Row, null);
                Row.ClearAssignment(field);
            }

            if (Row.IsAssigned(field))
                Row.ClearAssignment(field);
        }

        protected virtual void ValidateEditableFields(HashSet<Field> editable)
        {
            foreach (Field field in Row.GetFields())
            {
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

        protected virtual void ValidateRequired(HashSet<Field> editableFields)
        {
            var requiredFields = new HashSet<Field>();
            GetRequiredFields(required: requiredFields, editable: editableFields);
            Row.ValidateRequired(requiredFields);
        }

        protected virtual HashSet<Field> ValidateEditable()
        {
            var editableFields = new HashSet<Field>();
            GetEditableFields(editableFields);
            ValidateEditableFields(editableFields);
            return editableFields;
        }

        protected virtual void ValidateRequest()
        {
            var editableFields = ValidateEditable();
            ValidateRequired(editableFields);
            ValidateParent();
        }

        protected virtual void ValidateParent()
        {
            var parentIdRow = Row as IParentIdRow;
            if (parentIdRow == null)
                return;

            var parentId = parentIdRow.ParentIdField[Row];
            if (parentId == null)
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

        protected virtual void ExecuteInsert()
        {
            var idField = Row.IdField as Field;
            if (idField != null &&
                idField.Flags.HasFlag(FieldFlags.AutoIncrement))
            {
                Response.EntityId = new SqlInsert(Row)
                    .ExecuteAndGetID(Connection).Value;
                Row.IdField[Row] = Response.EntityId;
            }
            else
                new SqlInsert(Row).Execute(Connection);

            InvalidateCacheOnCommit();
        }

        protected virtual void DoGenericAudit()
        {
            var auditFields = new HashSet<Field>();
            GetEditableFields(auditFields);

            var auditRequest = GetAuditRequest(auditFields);
            if (auditRequest != null)
                AuditLogService.AuditInsert(Connection, RowRegistry.GetSchemaName(Row), auditRequest);
        }

        protected virtual void DoCaptureLog()
        {
            captureLogHandler.Log(this.UnitOfWork, this.Row, SecurityHelper.CurrentUserId, isDelete: false);
        }

        protected virtual void DoAudit()
        {
            if (!loggingInitialized)
            {
                var logTableAttr = typeof(TRow).GetCustomAttribute<CaptureLogAttribute>();
                if (logTableAttr != null)
                    captureLogHandler = new CaptureLogHandler<TRow>();

                hasAuditLogAttribute = typeof(TRow).GetCustomAttribute<AuditLogAttribute>() != null;
                loggingInitialized = true;
            }

            if (captureLogHandler != null)
                DoCaptureLog();
            else if (hasAuditLogAttribute)
                DoGenericAudit();
        }

        protected virtual void ValidatePermissions()
        {
            var attr = (OperationPermissionAttribute)typeof(TRow).GetCustomAttribute<InsertPermissionAttribute>(false) ??
                typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(false);

            if (attr != null)
            {
                if (attr.Permission.IsEmptyOrNull())
                    SecurityHelper.EnsureLoggedIn(RightErrorHandling.ThrowException);
                else
                    SecurityHelper.EnsurePermission(attr.Permission, RightErrorHandling.ThrowException);
            }
        }

        protected virtual BaseCriteria GetDisplayOrderFilter()
        {
            return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
        }

        protected virtual void SetDisplayOrderField()
        {
            var displayOrderRow = Row as IDisplayOrderRow;
            if (displayOrderRow != null)
            {
                var value = displayOrderRow.DisplayOrderField.AsObject(Row);
                if (value == null || Convert.ToInt32(value) <= 0)
                {
                    var filter = GetDisplayOrderFilter();
                    displayOrderRow.DisplayOrderField.AsObject(Row,
                        DisplayOrderHelper.GetNextValue(Connection, displayOrderRow, filter));
                }
                else
                    _displayOrderFix = true;
            }
        }

        public TCreateResponse Process(IUnitOfWork unitOfWork, TCreateRequest request)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            UnitOfWork = unitOfWork;

            Request = request;
            Response = new TCreateResponse();

            Row = request.Entity;
            if (Row == null)
                throw new ArgumentNullException("Entity");

            ValidatePermissions();
            ValidateRequest();
            SetInternalFields();
            BeforeInsert();
            ClearNonTableAssignments();
            ExecuteInsert();
            AfterInsert();
            DoAudit();
            OnReturn();

            return Response;
        }
    }

    public class CreateRequestHandler<TRow> : CreateRequestHandler<TRow, SaveRequest<TRow>, CreateResponse>
        where TRow : Row, IIdRow, new()
    {
    }

    public class DisplayOrderFilterHelper
    {
        public static BaseCriteria GetDisplayOrderFilterFor(Row row)
        {
            var flt = Criteria.Empty;
            var parentIdRow = row as IParentIdRow;
            if (parentIdRow != null)
                flt = flt & (new Criteria((Field)parentIdRow.ParentIdField) == Convert.ToInt64(((Field)parentIdRow.ParentIdField).AsObject(row)));

            var activeRow = row as IIsActiveRow;
            if (activeRow != null)
                flt = flt & new Criteria((Field)activeRow.IsActiveField) >= 0;

            return flt;
        }
    }
}
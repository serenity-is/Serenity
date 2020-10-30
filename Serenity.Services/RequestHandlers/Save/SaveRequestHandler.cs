using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Serenity.Services
{
    public class SaveRequestHandler<TRow, TSaveRequest, TSaveResponse> : ISaveRequestHandler, ISaveRequestProcessor
        where TRow : Row, IIdRow, new()
        where TSaveResponse : SaveResponse, new()
        where TSaveRequest : SaveRequest<TRow>, new()
    {
        private bool displayOrderFix;
        protected static IEnumerable<ISaveBehavior> cachedBehaviors;
        protected IEnumerable<ISaveBehavior> behaviors;

        public SaveRequestHandler()
        {
            this.StateBag = new Dictionary<string, object>();
            this.behaviors = GetBehaviors();
        }

        protected virtual IEnumerable<ISaveBehavior> GetBehaviors()
        {
            if (cachedBehaviors == null)
            {
                cachedBehaviors = RowSaveBehaviors<TRow>.Default.Concat(
                    this.GetType().GetCustomAttributes().OfType<ISaveBehavior>()).ToList();
            }

            return cachedBehaviors;
        }

        protected virtual void AfterSave()
        {
            HandleDisplayOrder(afterSave: true);

            foreach (var behavior in behaviors)
                behavior.OnAfterSave(this);
        }

        protected virtual void BeforeSave()
        {
            foreach (var behavior in behaviors)
                behavior.OnBeforeSave(this);
        }

        protected virtual void ClearNonTableAssignments()
        {
            foreach (var field in Row.GetFields())
            {
                if (Row.IsAssigned(field) && !field.IsTableField())
                    Row.ClearAssignment(field);
            }
        }

        protected virtual void PerformAuditing()
        {
            foreach (var behavior in behaviors)
                behavior.OnAudit(this);
        }

        protected virtual void ExecuteSave()
        {
            if (IsUpdate)
            {
                if (Row.IsAnyFieldAssigned)
                {
                    var idField = (Field)Row.IdField;

                    if (idField.IndexCompare(Old, Row) != 0)
                    {
                        var update = new SqlUpdate(Row.Table);
                        update.Set(Row);
                        update.Where(idField == new ValueCriteria(idField.AsSqlValue(Old)));
                        update.Execute(Connection, ExpectedRows.One);
                    }
                    else
                    {
                        Connection.UpdateById(Row);
                    }

                    Response.EntityId = idField.AsObject(Row);
                    InvalidateCacheOnCommit();
                }
            }
            else if (IsCreate)
            {
                var idField = Row.IdField as Field;
                if (!ReferenceEquals(null, idField) &&
                    idField.Flags.HasFlag(FieldFlags.AutoIncrement))
                {
                    var entityId = Connection.InsertAndGetID(Row);
                    Response.EntityId = entityId;
                    Row.IdField[Row] = entityId;
                }
                else
                {
                    Connection.Insert(Row);
                    if (!ReferenceEquals(null, idField))
                        Response.EntityId = ((Field)idField).AsObject(Row);
                }

                InvalidateCacheOnCommit();
            }
        }

        protected virtual BaseCriteria GetDisplayOrderFilter()
        {
            return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
        }

        protected virtual void GetEditableFields(HashSet<Field> editable)
        {
            var flag = IsCreate ? FieldFlags.Insertable : FieldFlags.Updatable;

            foreach (var field in Row.GetFields())
                if (field.Flags.HasFlag(flag))
                {
                    if ((IsCreate && (field.InsertPermission == null || Authorization.HasPermission(field.InsertPermission))) ||
                        (IsUpdate && (field.UpdatePermission == null || Authorization.HasPermission(field.UpdatePermission))))
                        editable.Add(field);
                }
        }

        protected virtual void GetRequiredFields(HashSet<Field> required, HashSet<Field> editable)
        {
            foreach (var field in Row.GetFields())
            {
                if (editable.Contains(field) &&
                    (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                    (field.Flags & FieldFlags.TrimToEmpty) != FieldFlags.TrimToEmpty)
                {
                    required.Add(field);
                }
            }
        }

        protected virtual void HandleDisplayOrder(bool afterSave)
        {
            var displayOrderRow = Row as IDisplayOrderRow;
            if (displayOrderRow == null)
                return;

            if (IsCreate && !afterSave)
            {
                var value = displayOrderRow.DisplayOrderField.AsObject(Row);
                if (value == null || Convert.ToInt32(value) <= 0)
                {
                    var filter = GetDisplayOrderFilter();
                    displayOrderRow.DisplayOrderField.AsObject(Row,
                        DisplayOrderHelper.GetNextValue(Connection, displayOrderRow, filter));
                }
                else
                    displayOrderFix = true;
            }
            else if (afterSave && 
                ((IsCreate && displayOrderFix) || 
                 (IsUpdate && displayOrderRow.DisplayOrderField[Old] != displayOrderRow.DisplayOrderField[Row])))
            {
                DisplayOrderHelper.ReorderValues(
                    connection: Connection,
                    row: displayOrderRow,
                    filter: GetDisplayOrderFilter(),
                    recordID: ((Field)Row.IdField).AsObject(Row),
                    newDisplayOrder: displayOrderRow.DisplayOrderField[Row].Value,
                    hasUniqueConstraint: false);
            }
        }

        protected virtual void HandleNonEditable(Field field)
        {
            if (IsUpdate && field.IndexCompare(Row, Old) == 0)
            {
                field.CopyNoAssignment(Old, Row);
                Row.ClearAssignment(field);
                return;
            }

            bool isNonTableField = ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign) ||
                  ((field.Flags & FieldFlags.Calculated) == FieldFlags.Calculated) ||
                  ((field.Flags & FieldFlags.NotMapped) == FieldFlags.NotMapped);

            if (IsUpdate)
            {
                if ((field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
                {
                    if (!isNonTableField)
                        throw DataValidation.ReadOnlyError(Row, field);

                    field.CopyNoAssignment(Old, Row);
                    Row.ClearAssignment(field);
                }
            }
            else if (IsCreate)
            {
                if (!field.IsNull(Row) &&
                    (field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
                {
                    if (!isNonTableField)
                        throw DataValidation.ReadOnlyError(Row, field);

                    field.AsObject(Row, null);
                    Row.ClearAssignment(field);
                }
            }

            if (Row.IsAssigned(field))
                Row.ClearAssignment(field);
        }

        protected virtual void LoadOldEntity()
        {
            if (!PrepareQuery().GetFirst(Connection))
            {
                var idField = (Field)(Row.IdField);
                var id = Request.EntityId != null ?
                    idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
                    : idField.AsObject(Row);

                throw DataValidation.EntityNotFoundError(Row, id);
            }
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in this.behaviors)
                behavior.OnReturn(this);
        }

        protected virtual SqlQuery PrepareQuery()
        {
            var idField = (Field)(Row.IdField);
            var id = Request.EntityId != null ?
                idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
                : idField.AsSqlValue(Row);

            var query = new SqlQuery()
                .Dialect(Connection.GetDialect())
                .From(Old)
                .SelectTableFields()
                .WhereEqual(idField, id);

            foreach (var behavior in behaviors)
                behavior.OnPrepareQuery(this, query);

            return query;
        }

        public TSaveResponse Process(IUnitOfWork unitOfWork, TSaveRequest request,
            SaveRequestType requestType = SaveRequestType.Auto)
        {
            StateBag.Clear();

            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            UnitOfWork = unitOfWork;

            Request = request;
            Response = new TSaveResponse();

            Row = request.Entity;
            if (Row == null)
                throw new ArgumentNullException("Entity");

            if (requestType == SaveRequestType.Auto)
            {
                if (((Field)Row.IdField).IsNull(Row))
                    requestType = SaveRequestType.Create;
                else
                    requestType = SaveRequestType.Update;
            }

            if (requestType == SaveRequestType.Update)
            {
                ValidateAndClearIdField();
                Old = new TRow();
                LoadOldEntity();
            }

            ValidateRequest();
            SetInternalFields();
            BeforeSave();

            ClearNonTableAssignments();
            ExecuteSave();

            AfterSave();

            PerformAuditing();

            OnReturn();
            return Response;
        }

        protected virtual void SetDefaultValue(Field field)
        {
            if (field.DefaultValue == null)
                return;

            field.AsObject(Row, field.ConvertValue(field.DefaultValue, CultureInfo.InvariantCulture));
        }

        protected virtual void SetDefaultValues()
        {
            foreach (var field in Row.GetTableFields())
            {
                if (Row.IsAssigned(field) || !field.IsNull(Row))
                    continue;

                SetDefaultValue(field);
            }

            var isActiveRow = Row as IIsActiveRow;
            if (isActiveRow != null &&
                !Row.IsAssigned(isActiveRow.IsActiveField))
                isActiveRow.IsActiveField[Row] = 1;
        }

        protected virtual void SetInternalFields()
        {
            if (IsCreate)
            {
                HandleDisplayOrder(afterSave: false);
                SetTrimToEmptyFields();
                SetDefaultValues();
            }

            foreach (var behaviour in this.behaviors)
                behaviour.OnSetInternalFields(this);
        }

        protected virtual void SetTrimToEmptyFields()
        {
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

        protected virtual void ValidateEditableFields(HashSet<Field> editable)
        {
            foreach (Field field in Row.GetFields())
            {
                if (IsUpdate && !Row.IsAssigned(field))
                {
                    field.CopyNoAssignment(Old, Row);
                    Row.ClearAssignment(field);
                    continue;
                }

                var stringField = field as StringField;
                if (!ReferenceEquals(null, stringField) &&
                    Row.IsAssigned(field))
                {
                    DataValidation.AutoTrim(Row, stringField);
                }

                if (!editable.Contains(field))
                    HandleNonEditable(field);
            }
        }

        protected virtual HashSet<Field> ValidateEditable()
        {
            var editableFields = new HashSet<Field>();
            GetEditableFields(editableFields);
            ValidateEditableFields(editableFields);
            return editableFields;
        }

        protected virtual void ValidateRequired(HashSet<Field> editableFields)
        {
            var requiredFields = new HashSet<Field>();
            GetRequiredFields(required: requiredFields, editable: editableFields);

            if (IsUpdate)
                Row.ValidateRequiredIfModified(requiredFields);
            else
                Row.ValidateRequired(requiredFields);
        }

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            var editableFields = ValidateEditable();
            ValidateRequired(editableFields);

            if (IsUpdate)
                ValidateIsActive();

            ValidateFieldValues();

            foreach (var behavior in this.behaviors)
                behavior.OnValidateRequest(this);
        }

        protected virtual void ValidateFieldValues()
        {
            var context = new RowValidationContext(this.Connection, this.Row);

            foreach (var field in Row.GetFields())
            {
                if (!Row.IsAssigned(field))
                    continue;

                if (field.CustomAttributes == null)
                    continue;

                var validators = field.CustomAttributes.OfType<ICustomValidator>();
                foreach (var validator in validators)
                {
                    context.Value = field.AsObject(this.Row);

                    var error = CustomValidate(context, field, validator);

                    if (error != null)
                        throw new ValidationError("CustomValidationError", field.PropertyName ?? field.Name, error);
                }
            }
        }

        protected virtual string CustomValidate(RowValidationContext context, Field field, ICustomValidator validator)
        {
            return validator.Validate(context);
        }

        protected virtual void ValidateIsActive()
        {
            var isActiveRow = Old as IIsActiveRow;
            if (isActiveRow != null &&
                isActiveRow.IsActiveField[Old] < 0)
                throw DataValidation.RecordNotActive(Old);

            var isDeletedRow = Old as IIsDeletedRow;
            if (isDeletedRow != null &&
                isDeletedRow.IsDeletedField[Old] == true)
                throw DataValidation.RecordNotActive(Old);
        }

        protected virtual void ValidateAndClearIdField()
        {
            var idField = (Field)(Row.IdField);
            if (Row.IsAssigned(idField))
                Row.ValidateRequired(idField);

            if ((idField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                Row.ClearAssignment(idField);
        }

        protected virtual void ValidatePermissions()
        {
            PermissionAttributeBase attr = null;
            
            if (IsUpdate) 
            {
                attr = typeof(TRow).GetCustomAttribute<UpdatePermissionAttribute>(true);
            }
            else if (IsCreate)
            {
                attr = typeof(TRow).GetCustomAttribute<InsertPermissionAttribute>(true);
            }

            attr = attr ?? (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(true) ??
                typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

            if (attr != null)
                Authorization.ValidatePermission(attr.Permission ?? "?");
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            BatchGenerationUpdater.OnCommit(this.UnitOfWork, Row);
        }

        SaveResponse ISaveRequestProcessor.Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type)
        {
            return Process(uow, (TSaveRequest)request, type);
        }

        public IDbConnection Connection { get { return UnitOfWork.Connection; } }

        public IUnitOfWork UnitOfWork { get; protected set; }       

        public TRow Old { get; protected set; }
        
        public TRow Row { get; protected set; }

        public bool IsCreate { get { return Old == null; } }

        public bool IsUpdate { get { return Old != null; } }

        public TSaveRequest Request { get; protected set; }

        public TSaveResponse Response { get; protected set; }

        ISaveRequest ISaveRequestHandler.Request { get { return this.Request; } }

        SaveResponse ISaveRequestHandler.Response { get { return this.Response; } }

        Row ISaveRequestHandler.Old { get { return this.Old; } }

        Row ISaveRequestHandler.Row { get { return this.Row; } }

        public IDictionary<string, object> StateBag { get; private set; }
    }

    public class SaveRequestHandler<TRow> : SaveRequestHandler<TRow, SaveRequest<TRow>, SaveResponse>
        where TRow : Row, IIdRow, new()
    {
    }

    public interface ISaveRequestProcessor
    {
        SaveResponse Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type);
    }
}

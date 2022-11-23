namespace Serenity.Services
{
    public class SaveRequestHandler<TRow, TSaveRequest, TSaveResponse> : ISaveRequestProcessor,
        ISaveHandler<TRow, TSaveRequest, TSaveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TSaveResponse : SaveResponse, new()
        where TSaveRequest : SaveRequest<TRow>, new()
    {
        private bool displayOrderFix;
        protected Lazy<ISaveBehavior[]> behaviors;

        public SaveRequestHandler(IRequestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            StateBag = new Dictionary<string, object>();
            behaviors = new Lazy<ISaveBehavior[]>(() => GetBehaviors().ToArray());
        }

        protected virtual IEnumerable<ISaveBehavior> GetBehaviors()
        {
            return Context.Behaviors.Resolve<TRow, ISaveBehavior>(GetType());
        }

        protected virtual void AfterSave()
        {
            HandleDisplayOrder(afterSave: true);

            foreach (var behavior in behaviors.Value)
                behavior.OnAfterSave(this);
        }

        protected virtual void BeforeSave()
        {
            foreach (var behavior in behaviors.Value)
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
            foreach (var behavior in behaviors.Value)
                behavior.OnAudit(this);
        }

        protected virtual void InvokeSaveAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                foreach (var behavior in behaviors.Value.OfType<ISaveExceptionBehavior>())
                    behavior.OnException(this, exception);

                throw;
            }
        }

        protected virtual void ExecuteSave()
        {
            if (IsUpdate)
            {
                if (Row.IsAnyFieldAssigned)
                {
                    var idField = Row.IdField;

                    if (idField.IndexCompare(Old, Row) != 0)
                    {
                        var update = new SqlUpdate(Row.Table);
                        update.Set(Row);
                        update.Where(idField == new ValueCriteria(idField.AsSqlValue(Old)));
                        InvokeSaveAction(() => update.Execute(Connection, ExpectedRows.One));
                    }
                    else
                    {
                        InvokeSaveAction(() => Connection.UpdateById(Row));
                    }

                    Response.EntityId = idField.AsObject(Row);
                    InvalidateCacheOnCommit();
                }
            }
            else if (IsCreate)
            {
                var idField = Row.IdField;
                if (idField is not null &&
                    idField.Flags.HasFlag(FieldFlags.AutoIncrement))
                {
                    InvokeSaveAction(() =>
                    {
                        var entityId = Connection.InsertAndGetID(Row);
                        Response.EntityId = entityId;
                        Row.IdField.AsObject(Row, Row.IdField.ConvertValue(entityId, CultureInfo.InvariantCulture));
                    });
                }
                else
                {
                    InvokeSaveAction(() =>
                    {
                        Connection.Insert(Row);
                    });

                    if (idField is not null)
                        Response.EntityId = idField.AsObject(Row);
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
                    if ((IsCreate && (field.InsertPermission == null ||
                            Permissions.HasPermission(field.InsertPermission))) ||
                        (IsUpdate && (field.UpdatePermission == null ||
                            Permissions.HasPermission(field.UpdatePermission))))
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
            if (Row is not IDisplayOrderRow displayOrderRow)
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
                    recordID: Row.IdField.AsObject(Row),
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
                        throw DataValidation.ReadOnlyError(field, Localizer);

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
                        throw DataValidation.ReadOnlyError(field, Localizer);

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
                var idField = Row.IdField;
                var id = Request.EntityId != null ?
                    idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
                    : idField.AsObject(Row);

                throw DataValidation.EntityNotFoundError(Row, id, Localizer);
            }
        }

        protected virtual void OnReturn()
        {
            foreach (var behavior in behaviors.Value)
                behavior.OnReturn(this);
        }

        protected virtual SqlQuery PrepareQuery()
        {
            var idField = Row.IdField;
            var id = Request.EntityId != null ?
                idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
                : idField.AsSqlValue(Row);

            var query = new SqlQuery()
                .Dialect(Connection.GetDialect())
                .From(Old)
                .SelectTableFields()
                .WhereEqual(idField, id);

            foreach (var behavior in behaviors.Value)
                behavior.OnPrepareQuery(this, query);

            return query;
        }

        public TSaveResponse Process(IUnitOfWork unitOfWork, TSaveRequest request,
            SaveRequestType requestType = SaveRequestType.Auto)
        {
            StateBag.Clear();

            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            Request = request ?? throw new ArgumentNullException(nameof(request));

            Response = new TSaveResponse();

            Row = (request.Entity ?? throw new ArgumentNullException(nameof(request.Entity))).Clone();

            if (requestType == SaveRequestType.Auto)
            {
                if (Row.IdField.IsNull(Row))
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
            else
                Old = null;

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

            if (Row is IIsActiveRow isActiveRow &&
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

            foreach (var behaviour in behaviors.Value)
                behaviour.OnSetInternalFields(this);
        }

        protected virtual void SetTrimToEmptyFields()
        {
            foreach (var field in Row.GetFields())
                if (!Row.IsAssigned(field) &&
                    field is StringField str &&
                    (field.Flags & FieldFlags.Insertable) == FieldFlags.Insertable &
                    (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull &
                    (field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                {
                    str[Row] = "";
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

                if (field is StringField stringField && Row.IsAssigned(field))
                    DataValidation.AutoTrim(Row, stringField);

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
                Row.ValidateRequiredIfModified(requiredFields, Localizer);
            else
                Row.ValidateRequired(requiredFields, Localizer);
        }

        protected virtual void ValidateRequest()
        {
            ValidatePermissions();

            var editableFields = ValidateEditable();
            ValidateRequired(editableFields);

            if (IsUpdate)
                ValidateIsActive();

            ValidateFieldValues();

            foreach (var behavior in behaviors.Value)
                behavior.OnValidateRequest(this);
        }

        protected virtual void ValidateFieldValues()
        {
            var context = new RowValidationContext(Connection, Row, Localizer);

            foreach (var field in Row.GetFields())
            {
                if (!Row.IsAssigned(field))
                    continue;

                var validators = field.CustomAttributes.OfType<ICustomValidator>();
                foreach (var validator in validators)
                {
                    context.Value = field.AsObject(Row);

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
            if (Old is IIsActiveRow isActiveRow &&
                isActiveRow.IsActiveField[Old] < 0)
                throw DataValidation.RecordNotActive(Old, Localizer);

            if (Old is IIsDeletedRow isDeletedRow &&
                isDeletedRow.IsDeletedField[Old] == true)
                throw DataValidation.RecordNotActive(Old, Localizer);
        }

        protected virtual void ValidateAndClearIdField()
        {
            var idField = Row.IdField;
            if (Row.IsAssigned(idField))
                Row.ValidateRequired(idField, Localizer);

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

            attr ??= (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(true) ??
                typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

            if (attr != null)
                Permissions.ValidatePermission(attr.Permission ?? "?", Localizer);
        }

        protected virtual void InvalidateCacheOnCommit()
        {
            Cache.InvalidateOnCommit(UnitOfWork, Row);
        }

        SaveResponse ISaveRequestProcessor.Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type)
        {
            return Process(uow, (TSaveRequest)request, type);
        }

        public TSaveResponse Create(IUnitOfWork uow, TSaveRequest request)
        {
            return Process(uow, request, SaveRequestType.Create);
        }

        public TSaveResponse Update(IUnitOfWork uow, TSaveRequest request)
        {
            return Process(uow, request, SaveRequestType.Update);
        }

        public ITwoLevelCache Cache => Context.Cache;
        public IRequestContext Context { get; private set; }
        public ITextLocalizer Localizer => Context.Localizer;
        public IPermissionService Permissions => Context.Permissions;
        public ClaimsPrincipal User => Context.User;

        public IDbConnection Connection => UnitOfWork.Connection;

        public IUnitOfWork UnitOfWork { get; protected set; }

        public TRow Old { get; protected set; }

        public TRow Row { get; protected set; }

        public bool IsCreate => Old == null;

        public bool IsUpdate => Old != null;

        public TSaveRequest Request { get; protected set; }

        public TSaveResponse Response { get; protected set; }

        ISaveRequest ISaveRequestHandler.Request => Request;

        SaveResponse ISaveRequestHandler.Response => Response;

        IRow ISaveRequestHandler.Old => Old;
        IRow ISaveRequestHandler.Row => Row;

        public IDictionary<string, object> StateBag { get; private set; }
    }
}

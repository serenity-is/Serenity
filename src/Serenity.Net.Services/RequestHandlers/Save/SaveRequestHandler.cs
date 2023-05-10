namespace Serenity.Services;

/// <summary>
/// Generic base class for save request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public class SaveRequestHandler<TRow, TSaveRequest, TSaveResponse> : ISaveRequestProcessor,
    ISaveHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private bool displayOrderFix;

    /// <summary>
    /// Lazy list of behaviors that is activated for this request
    /// </summary>
    protected Lazy<ISaveBehavior[]> behaviors;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException">context is null</exception>
    public SaveRequestHandler(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
        behaviors = new Lazy<ISaveBehavior[]>(() => GetBehaviors().ToArray());
    }

    /// <summary>
    /// Gets the list of save behaviors
    /// </summary>
    protected virtual IEnumerable<ISaveBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, ISaveBehavior>(GetType());
    }

    /// <summary>
    /// Called before executing the insert/update statement
    /// </summary>
    protected virtual void BeforeSave()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeSave(this);
    }

    /// <summary>
    /// Called after executing the insert/update statement
    /// </summary>
    protected virtual void AfterSave()
    {
        HandleDisplayOrder(afterSave: true);

        foreach (var behavior in behaviors.Value)
            behavior.OnAfterSave(this);
    }

    /// <summary>
    /// Clears assignment for fields that are marked as non table field,
    /// e.g. fields that are not mapped, view, expression etc.
    /// </summary>
    protected virtual void ClearNonTableAssignments()
    {
        foreach (var field in Row.GetFields())
        {
            if (Row.IsAssigned(field) && !field.IsTableField())
                Row.ClearAssignment(field);
        }
    }

    /// <summary>
    /// Performs auditing
    /// </summary>
    protected virtual void PerformAuditing()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnAudit(this);
    }

    /// <summary>
    /// Invokes the passed save action method
    /// </summary>
    /// <param name="action">Save action method</param>
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

    /// <summary>
    /// Executes the actual SQL save operation
    /// </summary>
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

    /// <summary>
    /// Gets the display order filter for current group, if the entity 
    /// implements <see cref="IDisplayOrderRow"/> interface
    /// </summary>
    protected virtual BaseCriteria GetDisplayOrderFilter()
    {
        return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
    }

    /// <summary>
    /// Gets the list of editable fields. These are fields that have
    /// Insertable (if Create), or Updatable (if Update) flags,
    /// and have the insert or update permission based on the type
    /// of the current operation.
    /// </summary>
    /// <param name="editable"></param>
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

    /// <summary>
    /// Gets the list of required fields based on editable list, and <see cref="FieldFlags.NotNull"/>
    /// </summary>
    /// <param name="required">List of required fields to populate</param>
    /// <param name="editable">Editable fields returned from
    /// <see cref="GetEditableFields(HashSet{Field})"/></param>
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

    /// <summary>
    /// Handles display order field calculation before and after save
    /// </summary>
    /// <param name="afterSave">True if called after save</param>
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

    /// <summary>
    /// Handles assignment to a non-editable field. If the field did not change
    /// in an update operation, it will be ignored by clearing the assignment. 
    /// For non-table fields it will also be ignored.
    /// </summary>
    /// <param name="field"></param>
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

    /// <summary>
    /// Loads the old entity for an update operation
    /// </summary>
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

    /// <summary>
    /// Called just before the response is returned
    /// </summary>
    protected virtual void OnReturn()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnReturn(this);
    }

    /// <summary>
    /// Prepares the query for selecting old record in an update operation
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Processes the save request. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <param name="requestType">Type of request, Create, Update or Auto</param>
    /// <exception cref="ArgumentNullException">unitofWork or request is null</exception>
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

    /// <summary>
    /// Sets the default value (<see cref="DefaultValueAttribute"/>) for the field
    /// </summary>
    /// <param name="field">Field</param>
    protected virtual void SetDefaultValue(Field field)
    {
        if (field.DefaultValue == null)
            return;

        field.AsObject(Row, field.ConvertValue(field.DefaultValue, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Sets the default values for all fields (<see cref="DefaultValueAttribute"/>)
    /// </summary>
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

    /// <summary>
    /// Sets values for internal fields
    /// </summary>
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

    /// <summary>
    /// Sets values for <see cref="FieldFlags.TrimToEmpty"/> fields.
    /// </summary>
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

    /// <summary>
    /// Validates editable fields, e.g. checks if the assigned fields
    /// are all in the editable set. Calls <see cref="HandleNonEditable(Field)"/>
    /// if not.
    /// </summary>
    /// <param name="editable">Set of editable fields</param>
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

    /// <summary>
    /// Validates editable fields
    /// </summary>
    /// <returns></returns>
    protected virtual HashSet<Field> ValidateEditable()
    {
        var editableFields = new HashSet<Field>();
        GetEditableFields(editableFields);
        ValidateEditableFields(editableFields);
        return editableFields;
    }

    /// <summary>
    /// Validates required fields
    /// </summary>
    /// <param name="editableFields">List of editable fields</param>
    protected virtual void ValidateRequired(HashSet<Field> editableFields)
    {
        var requiredFields = new HashSet<Field>();
        GetRequiredFields(required: requiredFields, editable: editableFields);

        if (IsUpdate)
            Row.ValidateRequiredIfModified(requiredFields, Localizer);
        else
            Row.ValidateRequired(requiredFields, Localizer);
    }

    /// <summary>
    /// Validates the request by checking insert / update permissions.
    /// </summary>
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

    /// <summary>
    /// Validates field values via <see cref="ICustomValidator"/> interface.
    /// </summary>
    /// <exception cref="ValidationError">One of the fields has an invalid value</exception>
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

    /// <summary>
    /// Calls custom validator
    /// </summary>
    /// <param name="context">Validation context</param>
    /// <param name="field">Field</param>
    /// <param name="validator">Custom validator</param>
    /// <returns></returns>
    protected virtual string CustomValidate(RowValidationContext context, Field field, ICustomValidator validator)
    {
        return validator.Validate(context);
    }

    /// <summary>
    /// Validates that record is not soft deleted
    /// </summary>
    protected virtual void ValidateIsActive()
    {
        if (Old is IIsActiveRow isActiveRow &&
            isActiveRow.IsActiveField[Old] < 0)
            throw DataValidation.RecordNotActive(Old, Localizer);

        if (Old is IIsDeletedRow isDeletedRow &&
            isDeletedRow.IsDeletedField[Old] == true)
            throw DataValidation.RecordNotActive(Old, Localizer);
    }

    /// <summary>
    /// Validates and clears the ID field
    /// </summary>
    protected virtual void ValidateAndClearIdField()
    {
        var idField = Row.IdField;
        if (Row.IsAssigned(idField))
            Row.ValidateRequired(idField, Localizer);

        if ((idField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
            Row.ClearAssignment(idField);
    }

    /// <summary>
    /// Validates user permissions by checking <see cref="InsertPermissionAttribute"/>
    /// and <see cref="UpdatePermissionAttribute"/>, and <see cref="ModifyPermissionAttribute"/>
    /// or <see cref="ReadPermissionAttribute" /> if others are not found.
    /// </summary>
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

    /// <summary>
    /// Attaches an handler to unit of work's OnCommit event to invalidate 
    /// the cache items related to this row's cache group
    /// </summary>
    protected virtual void InvalidateCacheOnCommit()
    {
        Cache.InvalidateOnCommit(UnitOfWork, Row);
    }

    SaveResponse ISaveRequestProcessor.Process(IUnitOfWork uow, ISaveRequest request, SaveRequestType type)
    {
        return Process(uow, (TSaveRequest)request, type);
    }

    /// <inheritdoc/>
    public TSaveResponse Create(IUnitOfWork uow, TSaveRequest request)
    {
        return Process(uow, request, SaveRequestType.Create);
    }

    /// <inheritdoc/>
    public TSaveResponse Update(IUnitOfWork uow, TSaveRequest request)
    {
        return Process(uow, request, SaveRequestType.Update);
    }

    /// <summary>
    /// Gets the two level cache from the request context
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context
    /// </summary>
    public IRequestContext Context { get; private set; }

    /// <summary>
    /// Gets localizer from the request context
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets permission service from the request context
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets current user from the request context
    /// </summary>
    public ClaimsPrincipal User => Context.User;

    /// <summary>
    /// Gets connection
    /// </summary>
    public IDbConnection Connection => UnitOfWork.Connection;

    /// <summary>
    /// Gets the current unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork { get; protected set; }

    /// <summary>
    /// The old entity for update
    /// </summary>
    public TRow Old { get; protected set; }

    /// <summary>
    /// The inserted entity for Create and new entity for Update
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Returns true if this is a Create operation
    /// </summary>
    public bool IsCreate => Old == null;

    /// <summary>
    /// Returns true if this is an Update operation
    /// </summary>
    public bool IsUpdate => Old != null;

    /// <summary>
    /// Request object
    /// </summary>
    public TSaveRequest Request { get; protected set; }

    /// <summary>
    /// Response object
    /// </summary>
    public TSaveResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; }

    ISaveRequest ISaveRequestHandler.Request => Request;
    SaveResponse ISaveRequestHandler.Response => Response;
    IRow ISaveRequestHandler.Old => Old;
    IRow ISaveRequestHandler.Row => Row;

}

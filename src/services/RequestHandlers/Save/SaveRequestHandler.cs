namespace Serenity.Services;

/// <summary>
/// Generic base class for save request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public class SaveRequestHandler<TRow, TSaveRequest, TSaveResponse> :
    SaveRequestHandlerBase<TRow, TSaveRequest, TSaveResponse>, ISaveRequestProcessor,
    ISaveHandler<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    private bool displayOrderFix;

    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<ISaveBehaviorSync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public SaveRequestHandler(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<ISaveBehaviorSync[]>(() =>
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<ISaveBehavior, ISaveBehaviorAsync, ISaveBehaviorSync>(
                GetBehaviors(), behavior => new AsyncToSyncSaveBehaviorWrapper(behavior))]);
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
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is ISaveExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

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
                    Row.IdField.AsInvariant(Row, entityId);
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
    /// Prepares the query for selecting old record in an update operation.
    /// </summary>
    /// <returns>The prepared query.</returns>
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
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> or <paramref name="request"/> is <c>null</c>.</exception>
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
}

namespace Serenity.Services;

/// <summary>
/// Generic base class for undelete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TUndeleteRequest">Undelete request type</typeparam>
/// <typeparam name="TUndeleteResponse">Undelete response type</typeparam>
public class UndeleteRequestHandler<TRow, TUndeleteRequest, TUndeleteResponse> :
    UndeleteRequestHandlerBase<TRow, TUndeleteRequest, TUndeleteResponse>, IUndeleteRequestProcessor,
    IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IUndeleteBehaviorSync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public UndeleteRequestHandler(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IUndeleteBehaviorSync[]>(() =>
            BehaviorProviderExtensions.AutoWrapBehaviors<IUndeleteBehavior, IUndeleteBehaviorAsync, IUndeleteBehaviorSync>(
                GetBehaviors(), behavior => new AsyncToSyncUndeleteBehaviorWrapper(behavior)).ToArray());
    }

    /// <summary>
    /// Method that is executed before the actual SQL undelete operation.
    /// </summary>
    protected virtual void OnBeforeUndelete()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeUndelete(this);
    }

    /// <summary>
    /// Method that is executed after the actual SQL undelete operation
    /// </summary>
    protected virtual void OnAfterUndelete()
    {
        if (Row is IDisplayOrderRow displayOrderRow)
        {
            var filter = GetDisplayOrderFilter();
            DisplayOrderHelper.ReorderValues(Connection!, displayOrderRow, filter,
                Row.IdField!.AsObject(Row), displayOrderRow.DisplayOrderField[Row]!.Value, false);
        }

        foreach (var behavior in behaviors.Value)
            behavior.OnAfterUndelete(this);
    }

    /// <summary>
    /// Validates the parameters of the undelete request.
    /// </summary>
    protected virtual void ValidateRequest()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnValidateRequest(this);
    }

    /// <summary>
    /// Prepares the query used to select the existing record
    /// </summary>
    /// <param name="query">The query</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        query.SelectTableFields();

        foreach (var behavior in behaviors.Value)
            behavior.OnPrepareQuery(this, query);
    }

    /// <summary>
    /// Loads the entity that is going to be undeleted
    /// </summary>
    protected virtual void LoadEntity()
    {
        var idField = Row.IdField;
        var id = idField!.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        var query = new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Row)
            .WhereEqual(idField, id);

        PrepareQuery(query);

        if (!query.GetFirst(Connection))
            throw DataValidation.EntityNotFoundError(Row, id, Localizer);
    }

    /// <summary>
    /// Invokes the passed undelete action method
    /// </summary>
    /// <param name="action">Undelete action method</param>
    protected virtual void InvokeUndeleteAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IUndeleteExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Executes the actual SQL undelete/update operation
    /// </summary>
    protected virtual void ExecuteUndelete()
    {
        var idField = Row.IdField;
        var id = idField!.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;

        var update = new SqlUpdate(Row.Table)
            .WhereEqual(idField, id);

        if (isActiveDeletedRow != null)
        {
            update.Set(isActiveDeletedRow.IsActiveField, 1)
                .WhereEqual(isActiveDeletedRow.IsActiveField, -1);
        }
        else if (isDeletedRow != null)
        {
            update.Set(isDeletedRow.IsDeletedField, false)
                .WhereEqual(isDeletedRow.IsDeletedField, 1);
        }

        if (Row is IDeleteLogRow deleteLogRow)
        {
            update.Set(deleteLogRow.DeleteUserIdField, null)
                .Set(deleteLogRow.DeleteDateField, null);

            if (isActiveDeletedRow == null && isDeletedRow == null)
                update.Where(deleteLogRow.DeleteUserIdField.IsNotNull());
        }

        InvokeUndeleteAction(() =>
        {
            if (update.Execute(Connection) != 1)
                throw DataValidation.EntityNotFoundError(Row, id, Localizer);
        });

        InvalidateCacheOnCommit();
    }

    /// <summary>
    /// Performs auditing
    /// </summary>
    protected virtual void DoAudit()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnAudit(this);
    }

    /// <summary>
    /// The method that is called just before the response is returned.
    /// </summary>
    protected virtual void OnReturn()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnReturn(this);
    }

    /// <summary>
    /// Processes the undelete request. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> is <c>null</c>.</exception>
    public TUndeleteResponse Process(IUnitOfWork unitOfWork, TUndeleteRequest request)
    {
        StateBag.Clear();
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Request = request;
        Response = new TUndeleteResponse();

        if (request.EntityId == null)
            throw DataValidation.RequiredError("EntityId", Localizer);

        Row = new TRow();

        LoadEntity();
        ValidatePermissions();
        ValidateRequest();

        if (!IsDeleted())
            Response.WasNotDeleted = true;
        else
        {
            OnBeforeUndelete();

            ExecuteUndelete();

            OnAfterUndelete();

            DoAudit();
        }

        OnReturn();

        return Response;
    }

    UndeleteResponse IUndeleteRequestProcessor.Process(IUnitOfWork uow, UndeleteRequest request)
    {
        return Process(uow, (TUndeleteRequest)request);
    }

    /// <inheritdoc/>
    public TUndeleteResponse Undelete(IUnitOfWork uow, TUndeleteRequest request)
    {
        return Process(uow, request);
    }
}

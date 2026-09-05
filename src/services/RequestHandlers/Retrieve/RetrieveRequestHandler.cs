namespace Serenity.Services;

/// <summary>
/// Generic base class for retrieve request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TRetrieveRequest">Retrieve request type</typeparam>
/// <typeparam name="TRetrieveResponse">Retrieve response type</typeparam>
public class RetrieveRequestHandler<TRow, TRetrieveRequest, TRetrieveResponse> :
    RetrieveRequestHandlerBase<TRow, TRetrieveRequest, TRetrieveResponse>, IRetrieveRequestProcessor,
    IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IRetrieveBehaviorSync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public RetrieveRequestHandler(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IRetrieveBehaviorSync[]>(() =>
            BehaviorProviderExtensions.AutoWrapBehaviors<IRetrieveBehavior, IRetrieveBehaviorAsync, IRetrieveBehaviorSync>(
                GetBehaviors(), behavior => new AsyncToSyncRetrieveBehaviorWrapper(behavior)).ToArray());
    }

    /// <summary>
    /// Prepares query by selecting fields.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        SelectFields(query);

        foreach (var behavior in behaviors.Value)
            behavior.OnPrepareQuery(this, query);
    }

    /// <summary>
    /// Called before executing the retrieve query
    /// </summary>
    protected virtual void OnBeforeExecuteQuery()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeExecuteQuery(this);
    }

    /// <summary>
    /// Called after executing the retrieve query
    /// </summary>
    protected virtual void OnAfterExecuteQuery()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnAfterExecuteQuery(this);
    }

    /// <summary>
    /// Called just before returning the response
    /// </summary>
    protected virtual void OnReturn()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnReturn(this);
    }

    /// <summary>
    /// Validates the request by checking permissions.
    /// </summary>
    protected virtual void ValidateRequest()
    {
        ValidatePermissions();

        foreach (var behavior in behaviors.Value)
            behavior.OnValidateRequest(this);
    }

    /// <summary>
    /// Executes the query and sets the response entity if found.
    /// </summary>
    /// <exception cref="ValidationError">If entity is not found</exception>
    protected virtual void ExecuteQuery()
    {
        try
        {
            if (Query.GetFirst(Connection))
                Response.Entity = Row;
            else
                throw DataValidation.EntityNotFoundError(Row, Request.EntityId, Localizer);
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IRetrieveExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Processes the retrieve request. This is the entry point for the handler.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public TRetrieveResponse Process(IDbConnection connection, TRetrieveRequest request)
    {
        StateBag.Clear();

        Connection = connection ?? throw new ArgumentNullException("connection");
        Request = request ?? throw new ArgumentNullException(nameof(request));

        if (request.EntityId == null)
            throw DataValidation.RequiredError("entityId", Localizer);

        ValidateRequest();

        Response = new TRetrieveResponse();
        Row = new TRow();

        Query = CreateQuery();

        PrepareQuery(Query);

        OnBeforeExecuteQuery();

        ExecuteQuery();

        OnAfterExecuteQuery();

        OnReturn();
        return Response;
    }

    IRetrieveResponse IRetrieveRequestProcessor.Process(IDbConnection connection, RetrieveRequest request)
    {
        return Process(connection, (TRetrieveRequest)request);
    }

    /// <inheritdoc/>
    public TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request)
    {
        return Process(connection, request);
    }
}

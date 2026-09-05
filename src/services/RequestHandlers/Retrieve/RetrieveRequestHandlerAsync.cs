namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous retrieve request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TRetrieveRequest">Retrieve request type</typeparam>
/// <typeparam name="TRetrieveResponse">Retrieve response type</typeparam>
public class RetrieveRequestHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse> :
    RetrieveRequestHandlerBase<TRow, TRetrieveRequest, TRetrieveResponse>, IRetrieveRequestProcessorAsync,
    IRetrieveHandlerAsync<TRow, TRetrieveRequest, TRetrieveResponse>
    where TRow : class, IRow, new()
    where TRetrieveRequest : RetrieveRequest
    where TRetrieveResponse : RetrieveResponse<TRow>, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IRetrieveBehaviorAsync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public RetrieveRequestHandlerAsync(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IRetrieveBehaviorAsync[]>(() =>
            BehaviorProviderExtensions.AutoWrapBehaviors<IRetrieveBehavior, IRetrieveBehaviorSync, IRetrieveBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncRetrieveBehaviorWrapper(behavior)).ToArray());
    }

    /// <summary>
    /// Prepares query by selecting fields.
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task PrepareQueryAsync(SqlQuery query, CancellationToken cancellationToken = default)
    {
        SelectFields(query);

        foreach (var behavior in behaviors.Value)
            await behavior.OnPrepareQueryAsync(this, query, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Called before executing the retrieve query
    /// </summary>
    protected virtual async Task OnBeforeExecuteQueryAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnBeforeExecuteQueryAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Called after executing the retrieve query
    /// </summary>
    protected virtual async Task OnAfterExecuteQueryAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnAfterExecuteQueryAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Called just before returning the response
    /// </summary>
    protected virtual async Task OnReturnAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnReturnAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the request by checking permissions.
    /// </summary>
    protected virtual async Task ValidateRequestAsync(CancellationToken cancellationToken = default)
    {
        ValidatePermissions();

        foreach (var behavior in behaviors.Value)
            await behavior.OnValidateRequestAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the query and sets the response entity if found.
    /// </summary>
    /// <exception cref="ValidationError">If entity is not found</exception>
    protected virtual async Task ExecuteQueryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await Query.GetFirstAsync(Connection, cancellationToken).ConfigureAwait(false))
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
    /// Processes the retrieve request asynchronously. This is the entry point for the handler.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public async Task<TRetrieveResponse> ProcessAsync(IDbConnection connection, TRetrieveRequest request,
        CancellationToken cancellationToken = default)
    {
        StateBag.Clear();

        Connection = connection ?? throw new ArgumentNullException("connection");
        Request = request ?? throw new ArgumentNullException(nameof(request));

        if (request.EntityId == null)
            throw DataValidation.RequiredError("entityId", Localizer);

        await ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        Response = new TRetrieveResponse();
        Row = new TRow();

        Query = CreateQuery();

        await PrepareQueryAsync(Query, cancellationToken).ConfigureAwait(false);

        await OnBeforeExecuteQueryAsync(cancellationToken).ConfigureAwait(false);

        await ExecuteQueryAsync(cancellationToken).ConfigureAwait(false);

        await OnAfterExecuteQueryAsync(cancellationToken).ConfigureAwait(false);

        await OnReturnAsync(cancellationToken).ConfigureAwait(false);
        return Response;
    }

    async Task<IRetrieveResponse> IRetrieveRequestProcessorAsync.ProcessAsync(IDbConnection connection, RetrieveRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessAsync(connection, (TRetrieveRequest)request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TRetrieveResponse> RetrieveAsync(IDbConnection connection, TRetrieveRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(connection, request, cancellationToken);
    }
}

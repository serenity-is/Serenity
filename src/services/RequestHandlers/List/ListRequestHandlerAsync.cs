namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous list request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public class ListRequestHandlerAsync<TRow, TListRequest, TListResponse> :
    ListRequestHandlerBase<TRow, TListRequest, TListResponse>, IListRequestProcessorAsync,
    IListHandlerAsync<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IListBehaviorAsync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public ListRequestHandlerAsync(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IListBehaviorAsync[]>(() =>
            BehaviorProviderExtensions.AutoWrapBehaviors<IListBehavior, IListBehaviorSync, IListBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncListBehaviorWrapper(behavior)).ToArray());
    }

    /// <inheritdoc/>
    protected override string MapFieldExpression(IField field, SqlQuery query)
    {
        foreach (var behavior in behaviors.Value)
        {
            if (behavior is IListMapFieldExpressionBehavior mapper &&
                mapper.MapFieldExpression(this, query, field) is string expression)
                return expression;
        }
        return null;
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
    /// Called before executing the list query
    /// </summary>
    protected virtual async Task OnBeforeExecuteQueryAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnBeforeExecuteQueryAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Called after executing the list query
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
    /// Applies all the filters including Request.EqualityFilter, Request.Criteria and
    /// Request.IncludeDeleted to the query.
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task ApplyFiltersAsync(SqlQuery query, CancellationToken cancellationToken = default)
    {
        ApplyEqualityFilter(query);
        ApplyCriteria(query);
        ApplyIncludeDeletedFilter(query);

        foreach (var behavior in behaviors.Value)
            await behavior.OnApplyFiltersAsync(this, query, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the query sets values / entities and total count.
    /// </summary>
    protected virtual async Task ExecuteQueryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Response.TotalCount = await Query.ForEachAsync(Connection, delegate ()
            {
                var clone = ProcessEntity(Row.Clone());
                if (clone == null)
                    return;

                if (DistinctFields != null)
                {
                    foreach (var field in DistinctFields)
                        Response.Values.Add(field.AsObject(clone));
                }
                else
                    Response.Entities.Add(clone);
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IListExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Processes the list request asynchronously. This is the entry point for the handler.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public async Task<TListResponse> ProcessAsync(IDbConnection connection, TListRequest request,
        CancellationToken cancellationToken = default)
    {
        StateBag.Clear();
        lookupAccessMode = false;
        ignoredEqualityFilters = null;
        Connection = connection ?? throw new ArgumentNullException("connection");
        Request = request ?? throw new ArgumentNullException(nameof(request));
        await ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        Response = new TListResponse
        {
            Entities = []
        };

        Row = new TRow();

        var query = CreateQuery();
        Query = query;

        DistinctFields = GetDistinctFields();
        if (DistinctFields != null)
            Response.Values = [];

        await PrepareQueryAsync(query, cancellationToken).ConfigureAwait(false);

        if (DistinctFields == null)
            ApplyKeyOrder(query);

        query.ApplySkipTakeAndCount(request.Skip, request.Take,
            request.ExcludeTotalCount || DistinctFields != null);

        ApplyContainsText(query, request.ContainsText);

        if (DistinctFields == null)
            ApplySort(query);

        await ApplyFiltersAsync(query, cancellationToken).ConfigureAwait(false);

        await OnBeforeExecuteQueryAsync(cancellationToken).ConfigureAwait(false);

        if (DistinctFields == null || DistinctFields.Length > 0)
        {
            await ExecuteQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // mark response to specify that one or more fields are invalid
            Response.Values = null;
        }

        Response.SetSkipTakeTotal(query);

        await OnAfterExecuteQueryAsync(cancellationToken).ConfigureAwait(false);

        await OnReturnAsync(cancellationToken).ConfigureAwait(false);

        return Response;
    }

    async Task<IListResponse> IListRequestProcessorAsync.ProcessAsync(IDbConnection connection, ListRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessAsync(connection, (TListRequest)request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TListResponse> ListAsync(IDbConnection connection, TListRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(connection, request, cancellationToken);
    }
}

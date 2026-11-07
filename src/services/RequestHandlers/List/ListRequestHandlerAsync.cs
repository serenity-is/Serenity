namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous list request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public class ListRequestHandlerAsync<TRow, TListRequest, TListResponse> :
    ListRequestHandlerBase<TRow, TListRequest, TListResponse>, IListRequestProcessorAsync, IListQueryCaptureAsync,
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
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<IListBehavior, IListBehaviorSync, IListBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncListBehaviorWrapper(behavior))]);
    }

    /// <inheritdoc/>
    protected override string? MapFieldExpression(IField field, SqlQuery query)
    {
        foreach (var behavior in behaviors.Value)
        {
            if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IListMapFieldExpressionBehavior mapper &&
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
            int count = 0;
            Response.TotalCount = await Query.ForEachAsync(Connection, delegate ()
            {
                count++;
                var clone = ProcessEntity(Row.Clone());
                if (clone == null)
                    return;

                if (DistinctFields != null)
                {
                    foreach (var field in DistinctFields)
                        Response.Values!.Add(field.AsObject(clone)!);
                }
                else
                    Response.Entities.Add(clone);
            }, cancellationToken: cancellationToken).ConfigureAwait(false);
            rowsRead = count;
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
        rowsRead = null;
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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

        if (ExecutionMode == ListExecutionMode.QueryCapture)
        {
            ApplyContainsText(query, request.ContainsText);
            await ApplyFiltersAsync(query, cancellationToken).ConfigureAwait(false);
            await OnBeforeExecuteQueryAsync(cancellationToken).ConfigureAwait(false);
            return Response;
        }

        if (DistinctFields == null)
            ApplyKeyOrder(query);

        query.ApplyPagingParams(GetPagingParams(), out var pagingState);

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

        Response.SetSkipTakeTotal(query, pagingState, DistinctFields?.Length, rowsRead);

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

    /// <summary>
    /// Builds the query this handler would execute for the request, without executing it.
    /// The request is validated, and the query is prepared and filtered by the handler and
    /// its behaviors as usual, with <see cref="ListRequestHandlerBase{TRow, TListRequest, TListResponse}.ExecutionMode"/>
    /// set to <see cref="ListExecutionMode.QueryCapture"/>. The query has no key order,
    /// sort, paging or total count, as it is meant to be used as a source of another query.
    /// Anything the handler or its behaviors would do after the query is executed, e.g.
    /// in ProcessEntity, OnAfterExecuteQuery or OnReturn, does not happen.
    /// </summary>
    /// <param name="connection">Connection, which the handler and its behaviors may use
    /// to run their own queries while building the query</param>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query</returns>
    public async Task<SqlQuery> CaptureQueryAsync(IDbConnection connection, TListRequest request,
        CancellationToken cancellationToken = default)
    {
        ExecutionMode = ListExecutionMode.QueryCapture;
        try
        {
            await ProcessAsync(connection, request, cancellationToken).ConfigureAwait(false);
            return Query;
        }
        finally
        {
            ExecutionMode = ListExecutionMode.Normal;
        }
    }

    Task<SqlQuery> IListQueryCaptureAsync.CaptureQueryAsync(IDbConnection connection, ListRequest request,
        CancellationToken cancellationToken)
    {
        return CaptureQueryAsync(connection, (TListRequest)request, cancellationToken);
    }
}

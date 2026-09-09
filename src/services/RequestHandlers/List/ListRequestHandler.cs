using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Generic base class for list request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TListRequest">List request type</typeparam>
/// <typeparam name="TListResponse">List response type</typeparam>
public class ListRequestHandler<TRow, TListRequest, TListResponse> :
    ListRequestHandlerBase<TRow, TListRequest, TListResponse>, IListRequestProcessor,
    IListHandler<TRow, TListRequest, TListResponse>
    where TRow : class, IRow, new()
    where TListRequest : ListRequest
    where TListResponse : ListResponse<TRow>, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IListBehaviorSync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public ListRequestHandler(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IListBehaviorSync[]>(() =>
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<IListBehavior, IListBehaviorAsync, IListBehaviorSync>(
                GetBehaviors(), behavior => new AsyncToSyncListBehaviorWrapper(behavior))]);
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
    protected virtual void PrepareQuery(SqlQuery query)
    {
        SelectFields(query);

        foreach (var behavior in behaviors.Value)
            behavior.OnPrepareQuery(this, query);
    }

    /// <summary>
    /// Called before executing the list query
    /// </summary>
    protected virtual void OnBeforeExecuteQuery()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeExecuteQuery(this);
    }

    /// <summary>
    /// Called after executing the list query
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
    /// Applies all the filters including Request.EqualityFilter, Request.Criteria and
    /// Request.IncludeDeleted to the query.
    /// </summary>
    /// <param name="query">Query</param>
    protected virtual void ApplyFilters(SqlQuery query)
    {
        ApplyEqualityFilter(query);
        ApplyCriteria(query);
        ApplyIncludeDeletedFilter(query);

        foreach (var behavior in behaviors.Value)
            behavior.OnApplyFilters(this, query);
    }

    /// <summary>
    /// Executes the query sets values / entities and total count.
    /// </summary>
    protected virtual void ExecuteQuery()
    {
        try
        {
            Response.TotalCount = Query.ForEach(Connection, delegate ()
            {
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
            });
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
    /// Processes the list request. This is the entry point for the handler.
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public TListResponse Process(IDbConnection connection, TListRequest request)
    {
        StateBag.Clear();
        lookupAccessMode = false;
        ignoredEqualityFilters = null;
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        ValidateRequest();

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

        PrepareQuery(query);

        if (DistinctFields == null)
            ApplyKeyOrder(query);

        query.ApplySkipTakeAndCount(request.Skip, request.Take,
            request.ExcludeTotalCount || DistinctFields != null);

        ApplyContainsText(query, request.ContainsText);

        if (DistinctFields == null)
            ApplySort(query);

        ApplyFilters(query);

        OnBeforeExecuteQuery();

        if (DistinctFields == null || DistinctFields.Length > 0)
        {
            ExecuteQuery();
        }
        else
        {
            // mark response to specify that one or more fields are invalid
            Response.Values = null;
        }

        Response.SetSkipTakeTotal(query);

        OnAfterExecuteQuery();

        OnReturn();

        return Response;
    }

    IListResponse IListRequestProcessor.Process(IDbConnection connection, ListRequest request)
    {
        return Process(connection, (TListRequest)request);
    }

    /// <inheritdoc/>
    public TListResponse List(IDbConnection connection, TListRequest request)
    {
        return Process(connection, request);
    }
}
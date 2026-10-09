namespace Serenity.Services;

/// <summary>
/// Contains static helper methods for service handler queries.
/// </summary>
public static class ServiceQueryHelper
{
    /// <summary>
    /// Applies the sort order to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="sort">Sort field, ignored if null, empty or
    /// not a usable field (e.g. a field that is selected in the query
    /// or available in the row). Note that this does not do any permission checks,
    /// so they must be pre-handled separately by the caller.</param>
    /// <param name="descending">Descending flag</param>
    /// <exception cref="ArgumentNullException">query is null</exception>
    public static SqlQuery ApplySort(this SqlQuery query, string? sort, bool descending)
    {
        ArgumentNullException.ThrowIfNull(query);

        var ext = (ISqlQueryExtensible)query;

        sort = sort.TrimToNull();

        if (sort != null)
        {
            string? expr = ((IGetExpressionByName)query).GetExpression(sort);

            if (expr == null)
            {
                var row = ext.FirstIntoRow;
                if (row != null)
                {
                    var field = ((IRow)row).FindFieldByPropertyName(sort);
                    if (field is not null)
                    {
                        expr = ((IGetExpressionByName)query).GetExpression(field.Name);
                        expr ??= field.Expression;
                    }
                }
            }

            if (expr != null)
                query.OrderByFirst(expr, descending);
        }

        return query;
    }

    /// <summary>
    /// Applies sort order to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="sortBy">Sort order</param>
    public static SqlQuery ApplySort(this SqlQuery query, SortBy sortBy)
    {
        if (sortBy?.Field != null)
            return ApplySort(query, sortBy.Field, sortBy.Descending);

        return query;
    }

    /// <summary>
    /// Applies sort orders to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="sortByList">Sort orders</param>
    /// <param name="defaultSortBy">The default sort order</param>
    public static SqlQuery ApplySort(this SqlQuery query, IList<SortBy> sortByList, params SortBy[] defaultSortBy)
    {
        if (sortByList == null || sortByList.Count == 0)
            sortByList = defaultSortBy;

        if (sortByList != null)
            for (var i = sortByList.Count - 1; i >= 0; i--)
            {
                var sortBy = sortByList[i];
                if (sortBy?.Field != null)
                {
                    ApplySort(query, sortBy.Field, sortBy.Descending);
                }
            }

        return query;
    }

    /// <summary>
    /// Applies skip, take and exclude total count parameters to the query.
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="skip">Skip parameter</param>
    /// <param name="take">Take parameter</param>
    /// <param name="excludeTotalCount">ExcludeTotalCount flag</param>
    /// <returns>The query.</returns>
    public static SqlQuery ApplySkipTakeAndCount(this SqlQuery query, int skip, int take,
        bool excludeTotalCount)
    {
        query.Skip(skip).Take(take);
        if (!excludeTotalCount &&
            query.Take() > 0)
            query.CountRecords = true;
        return query;
    }

    /// <summary>
    /// Applies paging and count parameters, optionally requesting one additional row to detect
    /// whether more results are available. Whether the total count is calculated is resolved
    /// from <see cref="ListRequestPagingParams"/> (see
    /// <see cref="ListRequestPagingParams.ShouldExcludeTotalCount"/>).
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="paging">Paging parameters.</param>
    /// <param name="pagingState">Receives the selected paging strategy.</param>
    /// <returns>The query.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> or <paramref name="paging"/> is null.</exception>
    public static SqlQuery ApplyPagingParams(this SqlQuery query, ListRequestPagingParams paging,
        out ListRequestPagingState pagingState)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(paging);

        var excludeTotalCount = paging.ShouldExcludeTotalCount;
        var take = paging.Take;

        var countAvailable = query.CountRecords || (!excludeTotalCount && take > 0);
        var usesSentinel = paging.IncludeMore && !countAvailable && take > 0 && take < int.MaxValue;
        var queryTake = usesSentinel ? take + 1 : take;

        query.Skip(paging.Skip).Take(queryTake);
        if (!excludeTotalCount && query.Take() > 0)
            query.CountRecords = true;

        pagingState = new ListRequestPagingState(paging, query.Take(), usesSentinel);
        return query;
    }

    /// <summary>
    /// Applies the configured paging limits (<see cref="RequestHandlerSettings.DefaultPageSize"/>
    /// and <see cref="RequestHandlerSettings.MaxPageSize"/>) to
    /// <see cref="ListRequestPagingParams.Take"/>, changing it in place. A zero take is replaced
    /// by the default page size, then clamped to the maximum page size when set.
    /// </summary>
    /// <param name="paging">Paging parameters (required).</param>
    /// <param name="settings">Handler settings that provide the limits (required).</param>
    /// <param name="suppressed">True to bypass the configured limits.</param>
    /// <returns>The same paging parameters, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paging"/> or <paramref name="settings"/> is null.</exception>
    public static ListRequestPagingParams ApplyPagingLimits(this ListRequestPagingParams paging,
        RequestHandlerSettings settings, bool suppressed = false)
    {
        ArgumentNullException.ThrowIfNull(paging);
        ArgumentNullException.ThrowIfNull(settings);

        if (suppressed)
            return paging;

        var originalTake = paging.Take;

        if (paging.Take == 0 && settings.DefaultPageSize > 0)
            paging.Take = settings.DefaultPageSize;

        if (settings.MaxPageSize > 0 &&
            (paging.Take == 0 || paging.Take > settings.MaxPageSize))
            paging.Take = settings.MaxPageSize;

        // a take of zero (unpaged) that the limits turned into an actual page size should
        // still return the total, so assume ExcludeTotalCount = false when it was unspecified,
        // unless a More indicator was requested (the caller then expects More, not the total)
        if (originalTake == 0 && paging.Take > 0 &&
            paging.ExcludeTotalCount is null && !paging.IncludeMore)
            paging.ExcludeTotalCount = false;

        return paging;
    }

    /// <summary>
    /// Applies contains text criteria to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="containsText">Contains text</param>
    /// <param name="filter">Filter callback</param>
    public static SqlQuery ApplyContainsText(this SqlQuery query, string? containsText,
        Action<string, long?> filter)
    {
        containsText = containsText.TrimToNull();
        if (containsText == null)
            return query;

        long? parsedId;
        if (long.TryParse(containsText, out long l))
            parsedId = l;
        else
            parsedId = null;

        filter(containsText, parsedId);
        return query;
    }

    /// <summary>
    /// Creates a contains text criteria
    /// </summary>
    /// <param name="containsText">Contains text</param>
    /// <param name="textFields">The list of fields to search contains text in</param>
    public static BaseCriteria? GetContainsTextFilter(string? containsText, Criteria[] textFields)
    {
        containsText = containsText.TrimToNull();
        if (containsText != null && textFields.Length > 0)
        {
            var flt = Criteria.Empty;
            foreach (var field in textFields)
                flt |= field.Contains(containsText);
            flt = ~(flt);

            return flt;
        }
        return null;
    }

    /// <summary>
    /// Gets not deleted criteria for a row type, e.g. for 
    /// rows that support soft delete.
    /// </summary>
    /// <param name="row">Row instance</param>
    public static BaseCriteria? GetNotDeletedCriteria(IRow row)
    {
        if (row is IIsActiveDeletedRow isActiveDeletedRow)
        {
            var criteria = isActiveDeletedRow.IsActiveField >= 0;
            if ((isActiveDeletedRow.IsActiveField.Flags & FieldFlags.NotNull) != FieldFlags.NotNull)
                return isActiveDeletedRow.IsActiveField.IsNull() | criteria;

            return criteria;
        }

        if (row is IIsDeletedRow isDeletedRow)
        {
            var criteria = isDeletedRow.IsDeletedField == 0;
            if ((isDeletedRow.IsDeletedField.Flags & FieldFlags.NotNull) != FieldFlags.NotNull)
                return isDeletedRow.IsDeletedField.IsNull() | criteria;

            return criteria;
        }

        if (row is IDeleteLogRow deleteLogRow)
            return deleteLogRow.DeleteDateField.IsNull();

        return null;
    }

    /// <summary>
    /// Returns if row uses soft delete
    /// </summary>
    /// <param name="row">Row instance</param>
    public static bool UseSoftDelete(IRow row)
    {
        return row is IIsActiveDeletedRow ||
            row is IIsDeletedRow ||
            row is IDeleteLogRow;
    }
}
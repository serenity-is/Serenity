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
    /// or available in the row)</param>
    /// <param name="descending">Descending flag</param>
    /// <exception cref="ArgumentNullException">query is null</exception>
    public static SqlQuery ApplySort(this SqlQuery query, string sort, bool descending)
    {
        if (query == null)
            throw new ArgumentNullException("query");

        var ext = (ISqlQueryExtensible)query;

        sort = sort.TrimToNull();

        if (sort != null)
        {
            string expr = ((IGetExpressionByName)query).GetExpression(sort);

            if (expr == null)
            {
                var row = ext.FirstIntoRow;
                if (row != null)
                {
                    var field = ((IRow)ext.FirstIntoRow).FindFieldByPropertyName(sort);
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
        if (sortBy != null)
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
                if (sortBy != null)
                {
                    ApplySort(query, sortBy.Field, sortBy.Descending);
                }
            }

        return query;
    }

    /// <summary>
    /// Applies skip, take and exclude total count parameters to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="skip">Skip parameter</param>
    /// <param name="take">Take parameter</param>
    /// <param name="excludeTotalCount">ExcludeTotalCount flag</param>
    /// <returns></returns>
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
    /// Applies contains text criteria to the query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="containsText">Contains text</param>
    /// <param name="filter">Filter callback</param>
    public static SqlQuery ApplyContainsText(this SqlQuery query, string containsText,
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
    public static BaseCriteria GetContainsTextFilter(string containsText, Criteria[] textFields)
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
    public static BaseCriteria GetNotDeletedCriteria(IRow row)
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
            return deleteLogRow.DeleteUserIdField.IsNull();

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
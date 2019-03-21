using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Services
{
    public static class ServiceQueryHelper
    {
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
                        var field = ((Row)ext.FirstIntoRow).FindFieldByPropertyName(sort);
                        if (!ReferenceEquals(null, field))
                        {
                            expr = ((IGetExpressionByName)query).GetExpression(field.Name);
                            if (expr == null)
                                expr = field.Expression;
                        }
                    }
                }

                if (expr != null)
                    query.OrderByFirst(expr, descending);
            }

            return query;
        }

        public static SqlQuery ApplySort(this SqlQuery query, SortBy sortBy)
        {
            if (sortBy != null)
                return ApplySort(query, sortBy.Field, sortBy.Descending);

            return query;
        }

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

        public static SqlQuery ApplySkipTakeAndCount(this SqlQuery query, int skip, int take,
            bool excludeTotalCount)
        {
            query.Skip(skip).Take(take);
            if (!excludeTotalCount &&
                query.Take() > 0)
                query.CountRecords = true;
            return query;
        }

        public static SqlQuery ApplyContainsText(this SqlQuery query, string containsText,
            Action<string, Int64?> filter)
        {
            containsText = containsText.TrimToNull();
            if (containsText == null)
                return query;

            Int64? parsedId;
            Int64 l;
            if (Int64.TryParse(containsText, out l))
                parsedId = l;
            else
                parsedId = null;

            filter(containsText, parsedId);
            return query;
        }

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

        public static BaseCriteria GetNotDeletedCriteria(Row row)
        {
            var isActiveDeletedRow = row as IIsActiveDeletedRow;
            if (isActiveDeletedRow != null)
            {
                var criteria = isActiveDeletedRow.IsActiveField >= 0;
                if ((isActiveDeletedRow.IsActiveField.Flags & FieldFlags.NotNull) != FieldFlags.NotNull)
                    return isActiveDeletedRow.IsActiveField.IsNull() | criteria;

                return criteria;
            }

            var isDeletedRow = row as IIsDeletedRow;
            if (isDeletedRow != null)
            {
                var criteria = isDeletedRow.IsDeletedField == 0;
                if ((isDeletedRow.IsDeletedField.Flags & FieldFlags.NotNull) != FieldFlags.NotNull)
                    return isDeletedRow.IsDeletedField.IsNull() | criteria;

                return criteria;
            }
            
            var deleteLogRow = row as IDeleteLogRow;
            if (deleteLogRow != null)
                return ((Field)deleteLogRow.DeleteUserIdField).IsNull();

            return null;
        }

        public static bool UseSoftDelete(Row row)
        {
            return row is IIsActiveDeletedRow ||
                row is IIsDeletedRow ||
                row is IDeleteLogRow;
        }
    }
}
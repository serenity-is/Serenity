using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Services
{
    public static class QueryHelper
    {
        public static SqlQuery ApplySort(this SqlQuery query, string sort, bool descending)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            sort = sort.TrimToNull();

            if (sort != null)
            {
                string expr = query.GetExpression(sort);

                if (expr == null)
                {
                    var row = query.IntoRow;
                    if (row != null)
                    {
                        var field = query.IntoRow.FindFieldByPropertyName(sort);
                        if (field != null)
                            expr = query.GetExpression(field.Name);
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
            query.Limit(skip, take);
            if (!excludeTotalCount &&
                query.Take() > 0)
                query.CountRecords = true;
            return query;
        }

        public static SqlQuery ApplyContainsText(this SqlQuery query, string containsText,
            Criteria idField, params Criteria[] fields)
        {
            var flt = GetContainsTextFilter(containsText, idField, fields);
            query.Where(flt);
            return query;
        }

        public static BaseCriteria GetContainsTextFilter(string containsText, BaseCriteria idField, params Criteria[] fields)
        {
            var ctFilter = GetContainsTextFilter(containsText, fields);
            if (Object.ReferenceEquals(idField, null))
                return ctFilter;

            containsText = containsText.TrimToNull();
            if (containsText == null)
                return ctFilter;

            Int64 idValue;
            if (Int64.TryParse(containsText, out idValue))
                if (Object.ReferenceEquals(null, ctFilter) || ctFilter.IsEmpty)
                    ctFilter = idField == idValue;
                else
                    ctFilter = ~(idField == idValue | ~(ctFilter));

            return ctFilter;
        }

        public static BaseCriteria GetContainsTextFilter(string containsText, params Criteria[] fields)
        {
            containsText = containsText.TrimToNull();
            if (containsText != null && fields.Length > 0)
            {
                var flt = Criteria.Empty;
                foreach (var field in fields)
                    flt |= field.Contains(containsText);
                flt = ~(flt);

                return flt;
            }
            return null;
        }

        public static SqlQuery ApplyFilters(this SqlQuery query,
            BasicFilterBase filter,
            IList<FilterLine> filterLines,
            Row row,
            Func<BasicFilter, BaseCriteria> processCriteria,
            FilterFields filterFields)
        {
            if (Object.ReferenceEquals(filter, null) &&
                (filterLines == null ||
                 filterLines.Count == 0))
                return query;

            var converter = new BasicFilterStringConverter(query, row, processCriteria, filterFields);

            BaseCriteria where;

            if (!Object.ReferenceEquals(filter, null))
            {
                where = converter.Convert(filter);
                if (!where.IsEmpty)
                    query.Where(where);
            }

            if (filterLines != null &&
                filterLines.Count > 0)
            {
                var linesFilter = filterLines.ToBasicFilter();
                where = converter.Convert(linesFilter);
                if (!where.IsEmpty)
                    query.Where(where);
            }
            return query;
        }
    }
}
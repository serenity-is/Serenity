using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Services
{
    public static class QueryHelper
    {
        public static SqlSelect ApplySort(this SqlSelect query, string sort, bool descending)
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

        public static SqlSelect ApplySort(this SqlSelect query, SortBy sortBy)
        {
            if (sortBy != null)
                return ApplySort(query, sortBy.Field, sortBy.Descending);

            return query;
        }

        public static SqlSelect ApplySort(this SqlSelect query, IList<SortBy> sortByList, params SortBy[] defaultSortBy)
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

        public static SqlSelect ApplySkipTakeAndCount(this SqlSelect query, int skip, int take,
            bool excludeTotalCount)
        {
            query.Limit(skip, take);
            if (!excludeTotalCount &&
                query.Take() > 0)
                query.CountRecords = true;
            return query;
        }

        public static SqlSelect ApplyContainsText(this SqlSelect query, string containsText, int joinIndex, params Field[] fields)
        {
            containsText = containsText.TrimToNull();
            if (containsText != null)
            {
                var flt = new Filter();
                foreach (var field in fields)
                    flt |= new Filter(joinIndex, field).Contains(containsText);
                flt = ~(flt);
                query.Where(flt);
            }
            return query;
        }

        public static SqlSelect ApplyContainsText(this SqlSelect query,
            string containsText, params Field[] fields)
        {
            containsText = containsText.TrimToNull();
            if (containsText != null)
            {
                var flt = new Filter();
                foreach (var field in fields)
                {
                    if (field.Expression != null)
                    {
                        query.EnsureForeignJoin(field);
                        flt |= new Filter(field.Expression).Contains(containsText);
                    }
                    else
                        flt |= new Filter(0, field).Contains(containsText);
                }
                flt = ~(flt);
                query.Where(flt);
            }
            return query;
        }

        public static SqlSelect ApplyFilters(this SqlSelect query,
            BasicFilter filter,
            IList<FilterLine> filterLines,
            Row row,
            Func<BasicCriteria, Filter> processCriteria,
            FilterFields filterFields)
        {
            if (Object.ReferenceEquals(filter, null) &&
                (filterLines == null ||
                 filterLines.Count == 0))
                return query;

            var converter = new BasicFilterStringConverter(query, row, processCriteria, filterFields);

            string where;

            if (!Object.ReferenceEquals(filter, null))
            {
                where = converter.Convert(filter);
                if (!where.IsEmptyOrNull())
                    query.Where(where);
            }

            if (filterLines != null &&
                filterLines.Count > 0)
            {
                var linesFilter = filterLines.ToBasicFilter();
                where = converter.Convert(linesFilter);
                if (!where.IsEmptyOrNull())
                    query.Where(where);
            }
            return query;
        }
    }
}
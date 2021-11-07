using System;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlQuery
    {
        /// <summary>
        ///   Formats SQL Query as string. If paging is used and skip requested, multiple queries 
        ///   might be created one after each other.</summary>
        /// <returns>
        ///   Formatted SELECT statement</returns>
        public override string ToString()
        {
            if (dialect is ISqlQueryToString dialectWithToString)
            {
                var queryElements = new SqlQueryElements
                {
                    AliasExpressions = aliasExpressions,
                    AliasWithJoins = aliasWithJoins,
                    Columns = columns,
                    CountRecords = countRecords,
                    Distinct = distinct,
                    ForJson = forJson,
                    ForXml = forXml,
                    From = from,
                    GroupBy = groupBy,
                    Having = having,
                    Into = into,
                    IntoIndex = intoIndex,
                    OrderBy = orderBy,
                    Skip = skip,
                    Take = take,
                    UnionQuery = unionQuery,
                    UnionType = unionType,
                    Where = where
                };

                return dialectWithToString.SqlQueryToString(this, queryElements);
            }

            var sb = new StringBuilder();

            if (unionQuery != null)
            {
                sb.Append(unionQuery.ToString());
                sb.Append("\n\n");
                sb.Append(dialect.UnionKeyword(unionType));
                sb.Append("\n\n");
            }

            // sub queries should be enclosed in paranthesis
            if (parent != null && !omitParens)
                sb.Append("(");

            if (skip > 0 && orderBy == null && !dialect.CanUseSkipKeyword && !dialect.UseRowNum)
                throw new InvalidOperationException("A query must be ordered by unique fields " +
                    "to be able to skip records!");

            // no extra filter yet
            string extraWhere = null;

            bool useSkipKeyword = skip > 0 && dialect.CanUseSkipKeyword;
            bool useOffset = skip > 0 && !useSkipKeyword && dialect.CanUseOffsetFetch;
            bool useRowNum = (skip > 0 || take > 0) && dialect.UseRowNum;
            bool useRowNumber = skip > 0 && !useSkipKeyword && !useOffset && !useRowNum && dialect.CanUseRowNumber;
            bool useSecondQuery = skip > 0 && !useSkipKeyword && !useOffset && !useRowNumber;

            // skip requested?
            if (useRowNumber || useRowNum || useSecondQuery)
            {
                if (useRowNumber || useRowNum)
                {
                    sb.Append("SELECT * FROM (\n");
                }
                else
                {
                    // this part is for servers that does not support paging at all (e.g. SQL 2000)
                    const string AssignCmd = "@Value{0} = {1}";
                    const string DeclareCmd = "DECLARE @Value{0} SQL_VARIANT;\n";
                    const string Equality = "(({0} IS NULL AND @Value{1} IS NULL) OR ({0} = @Value{1}))";
                    const string Greater = "(({0} IS NOT NULL AND @Value{1} IS NULL) OR ({0} > @Value{1}))";
                    const string LessThan = "(({0} IS NULL AND @Value{1} IS NOT NULL) OR ({0} < @Value{1}))";

                    // to skip records and bring back remaining, first select only keys of SKIP records in
                    // the order requested. Find the sorted field values of last record.

                    // For example, if actual query is
                    // SELECT ID, A, B, C, D FROM TABLE WHERE X > Y ORDER BY ID. 
                    // If SKIP is 5, need to skip first 5 records and continue from 6th.
                    // For this, we need to execute a query like
                    // DECLARE @ID SQL_VARIANT; SELECT TOP 5 @ID = ID FROM TABLE ORDER BY ID
                    // and get ID of the 5th record. To get the records after 5th:
                    // SELECT ID, A, B, C, D FROM TABLE WHERE ID > @ID AND X > Y ORDER BY ID
                    // Here, @ID is the ID of the 5th record determined with prior query.

                    // When there is more than one ordered field, for example when order is DATE, ID,
                    // again we first find the DATE and ID values of the last record.
                    // But, we can't simply write DATE > @DATE AND ID > @ID. 
                    // Because this means query might not bring back records which has a bigger ID, even 
                    // if their DATE value is smaller. The criteria that should be used is:
                    // (DATE > @DATE) OR (DATE = @DATE AND ID > @ID)

                    // Null is also another problem, as NULL comparison with any other value returns NULL.
                    // To eliminate NULL comparison problems the final criteria should be:
                    // ((DATE IS NOT NULL AND @DATE IS NULL) OR (DATE > @DATE)) OR 
                    // ((ID IS NULL AND @ID IS NULL) OR (ID = @ID))
                    // In the first line, NULL values are assumed to be sorted before NOT NULL values,
                    // (which might not be the case for a few databases like FIREBIRD)
                    // In the second line, we assume two NULL values are equal in order.

                    // for second part of the query, e.g. records after first SKIP records
                    var check = new StringBuilder();

                    // sorted field names minus DESC
                    var order = new string[orderBy.Count];

                    // descending flag for sorted field names
                    var desc = new bool[orderBy.Count];

                    // scan all order list
                    for (int i = 0; i < orderBy.Count; i++)
                    {
                        // declare a SQL_VARIANT variable for all sorted fields
                        sb.AppendFormat(DeclareCmd, i);

                        string o = orderBy[i];
                        desc[i] = o.EndsWith(SqlKeywords.Desc, StringComparison.OrdinalIgnoreCase);

                        if (desc[i])
                            order[i] = o.Substring(0, o.Length - SqlKeywords.Desc.Length);
                        else
                            order[i] = o;
                    }

                    sb.Append(SqlKeywords.Select);
                    if (distinct)
                        sb.Append(SqlKeywords.Distinct);

                    sb.Append(dialect.TakeKeyword);
                    sb.Append(' ');
                    sb.Append(skip);
                    sb.Append(' ');

                    // @Value1 = SortedField1, @Value2 = SortedField2...
                    for (int i = 0; i < order.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        sb.AppendFormat(AssignCmd, i, order[i]);
                    }

                    AppendFromWhereOrderByGroupByHaving(sb, null, true);

                    sb.Append(";\n");

                    // the criteria to find records after the last one. it gets complexer as number of
                    // sorted fields increase. For example, for A, B, C, D sorted fields:
                    // (A > @A) OR 
                    // (A = @A AND B > @B) OR 
                    // (A = @A AND B = @B AND C > @C) OR
                    // (A = @A AND B = @B AND C = @C AND D > @D)
                    // NULL is ignored here for simplicity. As seen above, every line contains one more 
                    // comparison the the one before that. First line number - 1 is equality and last 
                    // one is greater than operator (or less than if descending)

                    // opening paren of all lines
                    check.Append('(');
                    for (int statement = 0; statement < order.Length; statement++)
                    {
                        if (statement > 0)
                            check.Append(" OR ");

                        // opening paren for this line
                        check.Append('(');

                        // write equality for line number minus 1 comparisons
                        for (int equality = 0; equality < statement; equality++)
                        {
                            if (equality > 0)
                                check.Append(" AND ");

                            // add equality operator with null checks
                            check.AppendFormat(Equality, order[equality], equality);
                        }

                        // add AND before GT/LT comparison. first line has no equality so no need for AND
                        if (statement > 0)
                            check.Append(" AND ");

                        if (desc[statement])
                            check.AppendFormat(LessThan, order[statement], statement);
                        else
                            check.AppendFormat(Greater, order[statement], statement);

                        check.Append(')');
                    }

                    check.Append(')');

                    // determine this statement as filter for next query
                    extraWhere = check.ToString();
                }
            }

            // actual SELECT query starts here
            sb.Append(SqlKeywords.Select);

            if (distinct)
            {
                sb.Append(SqlKeywords.Distinct);
            }

            // add TOP N if number of records to fetch is limited
            if (take != 0 && (!useOffset) && (!useRowNum) && (useRowNumber || !dialect.UseTakeAtEnd))
            {
                sb.Append(dialect.TakeKeyword);
                sb.Append(' ');
                sb.Append(useRowNumber ? (skip + take) : take);
                sb.Append(' ');
            }

            if (useSkipKeyword)
            {
                sb.Append(dialect.SkipKeyword);
                sb.Append(' ');
                sb.Append(skip);
            }

            StringBuilder selCount = null;
            if (distinct)
                selCount = new StringBuilder();

            sb.Append('\n');

            // traverse selected columns
            for (int i = 0; i < columns.Count; i++)
            {
                var s = columns[i];

                if (i > 0)
                {
                    sb.Append(",\n");
                    if (distinct)
                        selCount.Append(',');
                }

                sb.Append(s.Expression);

                if (distinct)
                    selCount.Append(s.Expression);

                // write alias if any
                if (!string.IsNullOrEmpty(s.ColumnName))
                {
                    sb.Append(SqlKeywords.As);
                    var quoted = dialect.QuoteColumnAlias(s.ColumnName);
                    sb.Append(quoted);
                    if (distinct)
                    {
                        selCount.Append(SqlKeywords.As);
                        selCount.Append(quoted);
                    }
                }
            }

            if (useRowNumber || useRowNum)
            {
                if (columns.Count > 0)
                    sb.Append(", ");

                if (useRowNum && orderBy == null)
                    sb.Append("ROWNUM AS x__rownum__");
                else
                {
                    sb.Append("ROW_NUMBER() OVER (ORDER BY ");

                    if (orderBy != null)
                    {
                        for (int i = 0; i < orderBy.Count; i++)
                        {
                            if (i > 0)
                                sb.Append(", ");

                            sb.Append(orderBy[i]);
                        }
                    }

                    sb.Append(") AS ");
                    sb.Append(useRowNum ? "x__rownum__" : "__num__");
                }
            }

            // write remaining parts of the select query
            AppendFromWhereOrderByGroupByHaving(sb, extraWhere, true);

            if (useRowNumber)
            {
                sb.Append(") __results__ WHERE __num__ > ");
                sb.Append(skip);
            }

            if (useRowNum)
            {
                sb.Append(") WHERE x__rownum__ > " + skip);
                if (take > 0)
                    sb.Append(" AND ROWNUM <= " + take);
            }

            if (take != 0 && (!useRowNum) && (!useOffset) && !useRowNumber && dialect.UseTakeAtEnd)
            {
                sb.Append(' ');
                sb.Append(dialect.TakeKeyword);
                sb.Append(' ');
                sb.Append(take);
            }

            if (useOffset)
            {
                if (take == 0)
                    sb.Append(string.Format(dialect.OffsetFormat, skip, take));
                else
                    sb.Append(string.Format(dialect.OffsetFetchFormat, skip, take));
            }

            if (!string.IsNullOrEmpty(forXml))
            {
                sb.Append(" FOR XML ");
                sb.Append(forXml);
            }

            if (!string.IsNullOrEmpty(forJson))
            {
                sb.Append(" FOR JSON ");
                sb.Append(forJson);
            }

            if (countRecords)
            {
                if (!dialect.MultipleResultsets)
                    sb.Append("\n---\n"); // temporary fix till we find a better solution for firebird
                else
                    sb.Append(";\n");
                sb.Append(SqlKeywords.Select);
                sb.Append("count(*) ");

                if (distinct)
                {
                    sb.Append(SqlKeywords.From);
                    sb.Append('(');
                    sb.Append(SqlKeywords.Select);
                    sb.Append(SqlKeywords.Distinct);
                    sb.Append(selCount);
                }
                else if (groupBy != null && groupBy.Length > 0)
                {
                    sb.Append(SqlKeywords.From);
                    sb.Append('(');
                    sb.Append(SqlKeywords.Select);
                    sb.Append(" 1 as _alias_x_ ");
                }

                AppendFromWhereOrderByGroupByHaving(sb, null, false);

                if (distinct || (groupBy != null && groupBy.Length > 0))
                {
                    sb.Append(") _alias_");
                }
            }

            // sub queries should be enclosed in paranthesis
            if (parent != null && !omitParens)
                sb.Append(")");

            return sb.ToString();
        }

        private void AppendFromWhereOrderByGroupByHaving(StringBuilder sb, string extraWhere,
            bool includeOrderBy)
        {
            if (from.Length > 0)
            {
                sb.Append(SqlKeywords.From);
                sb.Append(from.ToString());
            }

            if (extraWhere != null || where != null)
            {
                sb.Append(SqlKeywords.Where);

                if (where != null)
                    sb.Append(@where);

                if (extraWhere != null)
                {
                    if (where != null)
                        sb.Append(" AND ");
                    sb.Append(extraWhere);
                }
            }

            if (groupBy != null)
            {
                sb.Append(SqlKeywords.GroupBy);
                sb.Append(groupBy);
            }

            if (having != null)
            {
                sb.Append(SqlKeywords.Having);
                sb.Append(having);
            }

            if (includeOrderBy && orderBy != null)
            {
                sb.Append(SqlKeywords.OrderBy);

                for (int i = 0; i < orderBy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");

                    sb.Append(orderBy[i]);
                }
            }
        }
    }
}
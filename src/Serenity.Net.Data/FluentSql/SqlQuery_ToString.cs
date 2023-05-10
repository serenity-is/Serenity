namespace Serenity.Data;

public partial class SqlQuery
{
    /// <summary>
    ///   Formats SQL Query as string. If paging is used and skip requested, multiple queries 
    ///   might be created one after each other.</summary>
    /// <returns>
    ///   Formatted SELECT statement</returns>
    public override string ToString()
    {
        return ToString(this, dialect);
    }

    IEnumerable<Column> ISqlQuery.Columns => columns;
    bool ISqlQuery.Distinct => distinct;
    string ISqlQuery.ForJson => forJson;
    string ISqlQuery.ForXml => forXml;
    string ISqlQuery.From => from?.ToString();
    string ISqlQuery.GroupBy => groupBy?.ToString();
    string ISqlQuery.Having => having?.ToString();
    bool ISqlQuery.OmitParens => omitParens;
    IEnumerable<string> ISqlQuery.OrderBy => (IEnumerable<string>)orderBy ?? Array.Empty<string>();
    IQueryWithParams ISqlQuery.Parent => parent;
    int ISqlQuery.Skip => skip;
    int ISqlQuery.Take => take;
    ISqlQuery ISqlQuery.UnionQuery => unionQuery;
    SqlUnionType ISqlQuery.UnionType => unionType;
    string ISqlQuery.Where => where?.ToString();

    /// <summary>
    ///   Formats SQL Query as string. If paging is used and skip requested, multiple queries 
    ///   might be created one after each other.</summary>
    /// <returns>
    ///   Formatted SELECT statement</returns>
    public static string ToString(ISqlQuery query, ISqlDialect dialect)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        if (dialect is ISqlQueryToString sqlQueryToString)
            return sqlQueryToString.ToString(query);

        var sb = new StringBuilder();

        if (query.UnionQuery != null)
        {
            sb.Append(query.UnionQuery.ToString());
            sb.Append("\n\n");
            sb.Append(dialect.UnionKeyword(query.UnionType));
            sb.Append("\n\n");
        }

        // sub queries should be enclosed in parenthesis
        if (query.Parent != null && !query.OmitParens)
            sb.Append("(");

        var skip = query.Skip;
        var take = query.Take;

        var orderBy = query.OrderBy.ToArray();

        string extraWhere = null;

        bool useSkipKeyword = skip > 0 && dialect.CanUseSkipKeyword;
        bool useOffset = (skip > 0 || (take > 0 && !dialect.CanUseSkipKeyword && dialect.UseRowNum)) && 
            !useSkipKeyword && dialect.CanUseOffsetFetch;
        bool useRowNum = (skip > 0 || take > 0) && !useOffset && dialect.UseRowNum;
        bool useRowNumber = skip > 0 && !useSkipKeyword && !useOffset && !useRowNum && dialect.CanUseRowNumber;
        bool useSecondQuery = skip > 0 && !useSkipKeyword && !useRowNum && !useOffset && !useRowNumber;

        void appendFromWhereOrderByGroupByHaving(string extraWhere, bool includeOrderBy)
        {
            if (!string.IsNullOrEmpty(query.From))
            {
                sb.Append(SqlKeywords.From);
                sb.Append(query.From);
            }

            if (extraWhere != null || !string.IsNullOrEmpty(query.Where))
            {
                sb.Append(SqlKeywords.Where);

                if (!string.IsNullOrEmpty(query.Where))
                    sb.Append(query.Where);

                if (extraWhere != null)
                {
                    if (!string.IsNullOrEmpty(query.Where))
                        sb.Append(" AND ");
                    sb.Append(extraWhere);
                }
            }

            if (!string.IsNullOrEmpty(query.GroupBy))
            {
                sb.Append(SqlKeywords.GroupBy);
                sb.Append(query.GroupBy);
            }

            if (!string.IsNullOrEmpty(query.Having))
            {
                sb.Append(SqlKeywords.Having);
                sb.Append(query.Having);
            }

            if (includeOrderBy && orderBy.Length > 0)
            {
                sb.Append(SqlKeywords.OrderBy);

                for (int i = 0; i < orderBy.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");

                    sb.Append(orderBy[i]);
                }
            }
        }

        // skip requested?
        if (useRowNumber || useRowNum || useSecondQuery)
        {
            if (useRowNumber || useRowNum)
            {
                sb.Append("SELECT * FROM (\n");
            }
            else
            {
                if (orderBy.Length == 0)
                    throw new InvalidOperationException("A query must be ordered by unique fields " +
                        "to be able to skip records!");

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
                var order = new string[orderBy.Length];

                // descending flag for sorted field names
                var desc = new bool[orderBy.Length];

                // scan all order list
                for (int i = 0; i < orderBy.Length; i++)
                {
                    // declare a SQL_VARIANT variable for all sorted fields
                    sb.AppendFormat(DeclareCmd, i);

                    string o = orderBy[i];
                    desc[i] = o.EndsWith(SqlKeywords.Desc, StringComparison.OrdinalIgnoreCase);

                    if (desc[i])
                        order[i] = o[..^SqlKeywords.Desc.Length];
                    else
                        order[i] = o;
                }

                sb.Append(SqlKeywords.Select);
                if (query.Distinct)
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

                appendFromWhereOrderByGroupByHaving(null, true);

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

        if (query.Distinct)
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
        if (query.Distinct)
            selCount = new StringBuilder();

        sb.Append('\n');

        var columns = query.Columns;

        // traverse selected columns
        int cidx = 0;
        foreach (var s in columns)
        {
            if (cidx > 0)
            {
                sb.Append(",\n");
                if (query.Distinct)
                    selCount.Append(',');
            }

            sb.Append(s.Expression);

            if (query.Distinct)
                selCount.Append(s.Expression);

            // write alias if any
            if (!string.IsNullOrEmpty(s.ColumnName))
            {
                sb.Append(SqlKeywords.As);
                var quoted = dialect.QuoteColumnAlias(s.ColumnName);
                sb.Append(quoted);
                if (query.Distinct)
                {
                    selCount.Append(SqlKeywords.As);
                    selCount.Append(quoted);
                }
            }

            cidx++;
        }

        if (useRowNumber || useRowNum)
        {
            if (columns.Any())
                sb.Append(", ");

            if (useRowNum && orderBy.Length == 0)
                sb.Append("ROWNUM AS x__rownum__");
            else
            {
                sb.Append("ROW_NUMBER() OVER (ORDER BY ");

                if (orderBy.Length > 0)
                {
                    for (var i = 0; i < orderBy.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(", ");

                        sb.Append(orderBy[i]);
                    }
                }

                sb.Append(") AS ");
                sb.Append(useRowNum ? "x__rownum__" : "x__num__");
            }
        }
        
        // write remaining parts of the select query
        appendFromWhereOrderByGroupByHaving(extraWhere, true);

        if (useRowNumber)
        {
            sb.Append(") x__results__ WHERE x__num__ > ");
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

        if (!string.IsNullOrEmpty(query.ForXml))
        {
            sb.Append(" FOR XML ");
            sb.Append(query.ForXml);
        }

        if (!string.IsNullOrEmpty(query.ForJson))
        {
            sb.Append(" FOR JSON ");
            sb.Append(query.ForJson);
        }

        if (query.CountRecords)
        {
            if (!dialect.MultipleResultsets)
                sb.Append("\n---\n"); // temporary fix till we find a better solution for firebird
            else
                sb.Append(";\n");
            sb.Append(SqlKeywords.Select);
            sb.Append("count(*) ");

            if (query.Distinct)
            {
                sb.Append(SqlKeywords.From);
                sb.Append('(');
                sb.Append(SqlKeywords.Select);
                sb.Append(SqlKeywords.Distinct);
                sb.Append(selCount);
            }
            else if (!string.IsNullOrEmpty(query.GroupBy))
            {
                sb.Append(SqlKeywords.From);
                sb.Append('(');
                sb.Append(SqlKeywords.Select);
                sb.Append(" 1 as x__alias__x ");
            }

            appendFromWhereOrderByGroupByHaving(null, false);

            if (query.Distinct || (!string.IsNullOrEmpty(query.GroupBy)))
            {
                sb.Append(") x__alias__");
            }
        }

        // sub queries should be enclosed in parenthesis
        if (query.Parent != null && !query.OmitParens)
            sb.Append(")");

        return sb.ToString();
    }


}
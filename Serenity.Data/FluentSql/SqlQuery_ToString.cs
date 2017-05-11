using System;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlQuery
    {
        /// <summary>
        /// Format the SqlSelect query and convert it to a SELECT query. For paging
        /// (if there are skipped records) multiple queries are created one after the other. </summary>
        /// <returns>
        /// Formatted SELECT expression </returns>
        public override string ToString()
        {
            // StringBuilder object to be used for formatting
            var sb = new StringBuilder();

            // sub queries should be enclosed in paranthesis
            if (this.parent != null)
                sb.Append("(");

            if (skip > 0 && orderBy == null && !dialect.CanUseSkipKeyword && !dialect.UseRowNum)
                throw new InvalidOperationException("A query must be ordered by unique fields " +
                    "to be able to skip records!");

            // no additional filters at this time
            string extraWhere = null;

            bool useSkipKeyword = skip > 0 && dialect.CanUseSkipKeyword;
            bool useOffset = skip > 0 && !useSkipKeyword && dialect.CanUseOffsetFetch;
            bool useRowNum = (skip > 0 || take > 0) && dialect.UseRowNum;
            bool useRowNumber = skip > 0 && !useSkipKeyword && !useOffset && !useRowNum && dialect.CanUseRowNumber;
            bool useSecondQuery = skip > 0 && !useSkipKeyword && !useOffset && !useRowNumber;

            // do you have the requested record to be skipped?
            if (useRowNumber || useRowNum || useSecondQuery)
            {
                if (useRowNumber || useRowNum)
                {
                    sb.Append("SELECT * FROM (\n");
                }
                else
                {
                    const string AssignCmd = "@Value{0} = {1}";
                    const string DeclareCmd = "DECLARE @Value{0} SQL_VARIANT;\n";
                    const string Equality = "(({0} IS NULL AND @Value{1} IS NULL) OR ({0} = @Value{1}))";
                    const string Greater = "(({0} IS NOT NULL AND @Value{1} IS NULL) OR ({0} > @Value{1}))";
                    const string LessThan = "(({0} IS NULL AND @Value{1} IS NOT NULL) OR ({0} < @Value{1}))";

                    // skip over the requested records and get the rest first
                    // run only to select key fields, skip to the desired record
                    // moving forward, we find the values ​​of the fields in this last record.

                    // For example our main question
                    // SELECT ID, A, B, C, D FROM TABLE WHERE X> Y ORDER BY ID. If SKIP 5 is specified
                    // the first 5 records should be skipped and continued from 6. For this,
                    // DECLARE @ID SQL_VARIANT; SELECT TOP 5 @ID = ID FROM TABLE ORDER BY ID
                    // is executed, and the ID of the 5th record is found. Records after 5
                    // If the query needs to be created to retrieve,
                    // SELECT ID, A, B, C, D FROM TABLE WHERE ID> @ID AND X> Y ORDER BY ID
                    // where @ID is the ID number of the 5th record.

                    // When there are multiple sorted fields, for example DATE, sort by ID fields
                    //, again the last values ​​of these fields are found. But this time,
                    // DATE> @DATE AND ID> @ID can not be written. Because a record by order is DATE value
                    // the larger the ID is, the lower the whatever. If this is the case,
                    // it needs to be big. The condition to be written,
                    // (DATE> @DATE) OR (DATE = @DATE AND ID> @ID).

                    // To avoid problems when comparing with null values ​​and not to break the sorting
                    // comparisons should be made taking into account Null values. The above expression is NULL
                    // Considering the situations:
                    // ((DATE IS NOT NULL AND @DATE IS NULL) OR (DATE> @DATE)) OR
                    // ((ID IS NULL AND @ ID IS NULL) OR (ID = @ ID))
                    //. NULL values ​​in the first row are not always null
                    // is considered a future before a value. On the second line, two NULL un
                    // are considered equal.

                    // condition to find records below records skipped for second query
                    // We will create StringBuilder
                    var check = new StringBuilder();

                    // array to hold sorted domain names with DESC at the end
                    var order = new string[orderBy.Count];

                    // array to hold fields in reverse order
                    var desc = new bool[orderBy.Count];

                    // scan entire sort list
                    for (int i = 0; i < orderBy.Count; i++)
                    {
                        // define an SQL_VARIANT variant for each sequential field
                        sb.AppendFormat(DeclareCmd, i);

                        // read sorted field
                        string o = orderBy[i];

                        // this is the reverse order if it ends with DESC, should be considered in the comparisons
                        desc[i] = o.EndsWith(SqlKeywords.Desc, StringComparison.OrdinalIgnoreCase);

                        // if it is in reverse order add DESC phrase to the list of ordered fields
                        if (desc[i])
                            order[i] = o.Substring(0, o.Length - SqlKeywords.Desc.Length);
                        else
                            order[i] = o;
                    }

                    // SELECT TOP ...
                    sb.Append(SqlKeywords.Select);
                    if (distinct)
                        sb.Append(SqlKeywords.Distinct);

                    sb.Append(dialect.TakeKeyword);
                    sb.Append(' ');
                    // Up to the number of records to be skipped
                    sb.Append(skip);
                    sb.Append(' ');

                    // Replace the field list with your own field list in SqlSelect
                    // Edit @ Value1 = SiraliAlan1, @ Value2 = SiraliAlan2.
                    for (int i = 0; i < order.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        sb.AppendFormat(AssignCmd, i, order[i]);
                    }

                    // Query other parts of SqlSelect
                    AppendFromWhereOrderByGroupByHaving(sb, null, true);

                    sb.Append(";\n");

                    // condition that will be useful for finding the last record, as the number of fields increases
                    // complicated. For example, in a 4-field sequence such as A, B, C, D,
                    // (A> @A) OR
                    // (A = @A AND B> @B) OR
                    // (A = @A AND B = @B AND C> @C) OR
                    // (A = @A AND B = @B AND C = @C AND D> @D)
                    // Here, NULL states are not written for simplicity. As you can see, one per line
                    // there is one more comparison before, their line number is -1
                    // equality, one of them is the size (smallness in opposite order).

                    // get all lines in common brackets
                    check.Append('(');
                    for (int statement = 0; statement < order.Length; statement++)
                    {
                        // put together OR after first line
                        if (statement > 0)
                            check.Append(" OR ");

                        // opening brace of this line
                        check.Append('(');

                        // from line number to one minus, equality conditions
                        for (int equality = 0; equality < statement; equality++)
                        {
                            // put together after first condition
                            if (equality > 0)
                                check.Append(" AND ");

                            // Write equality comparison that takes into account the null case
                            check.AppendFormat(Equality, order[equality], equality);
                        }

                        // put together before size or size condition.
                        // there is no equality in the first row, so there is no need for AND.
                        if (statement > 0)
                            check.Append(" AND ");

                        // size for this row, based on whether or not the sort is reversed
                        // or add a small case considering the null state
                        if (desc[statement])
                            check.AppendFormat(LessThan, order[statement], statement);
                        else
                            check.AppendFormat(Greater, order[statement], statement);

                        // closing brace of this line
                        check.Append(')');
                    }
                    // closing brace of all lines
                    check.Append(')');

                    // set this as an extra filter for the next main query
                    extraWhere = check.ToString();
                }
            }

            // actual SELECT query start
            sb.Append(SqlKeywords.Select);

            if (distinct)
            {
                sb.Append(SqlKeywords.Distinct);
            }

            // if the number of records to be fetched is limited, print it as TOP N per query
            if (take != 0 && (!useOffset) && (!useRowNum) && 
                ((useRowNumber && !dialect.UseTakeAtEnd) || 
                (!useRowNumber && (!dialect.UseTakeAtEnd || dialect.CanUseRowNumber))))
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
            // navigate the field list to be selected
            for (int i = 0; i < columns.Count; i++)
            {
                var s = columns[i];

                // comma delimited after first field
                if (i > 0)
                {
                    sb.Append(",\n");
                    if (distinct)
                        selCount.Append(',');
                }

                // Write domain
                sb.Append(s.Expression);

                if (distinct)
                    selCount.Append(s.Expression);

                // if an alias is assigned to the field, write it
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

                if (useRowNum)
                {
                    sb.Append("ROWNUM AS numberingofrow");
                }
                else
                {
                    sb.Append("ROW_NUMBER() OVER (ORDER BY ");

                    if (orderBy != null)
                        for (int i = 0; i < orderBy.Count; i++)
                        {
                            if (i > 0)
                                sb.Append(", ");

                            sb.Append(orderBy[i]);
                        }

                    sb.Append(") AS __num__");
                }

            }

            // Write the rest of the select query
            AppendFromWhereOrderByGroupByHaving(sb, extraWhere, !useRowNumber);

            if (useRowNumber)
            {
                sb.Append(") __results__ WHERE __num__ > " + skip);
                if (take > 0 && dialect.UseTakeAtEnd)
                    sb.Append(" AND __num__ <= " + (skip + take));
            }

            if (useRowNum)
            {
                sb.Append(") WHERE numberingofrow > " + skip);
                if (take > 0)
                    sb.Append(" AND ROWNUM <= " + take);
            }

            if (take != 0 && (!useRowNum) && (!useOffset) && !dialect.CanUseRowNumber && dialect.UseTakeAtEnd)
            {
                sb.Append(' ');
                sb.Append(dialect.TakeKeyword);
                sb.Append(' ');
                sb.Append(take);
            }

            if (useOffset)
            {
                if (take == 0)
                    sb.Append(String.Format(dialect.OffsetFormat, skip, take));
                else
                    sb.Append(String.Format(dialect.OffsetFetchFormat, skip, take));
            }

            if (!string.IsNullOrEmpty(forXml))
            {
                sb.Append(" FOR XML ");
                sb.Append(forXml);
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
            if (this.parent != null)
                sb.Append(")");

            // return select query
            return sb.ToString();
        }

        /// <summary>
        /// SqlSelect FROM, WHERE, ORDER BY, GROUP BY, HAVING to the given StringBuilder object
        /// sections, if specified, with an additional filter in mind. </summary>
        /// <param name="sb">
        /// The existing from, where ... sections of the SqlSelect object will be formatted and inserted
        /// StringBuilder object. </param>
        /// <param name="extraWhere">
        /// Extra filter to be added by ANDing WHERE conditions if specified. </param>
        /// <param name="includeOrderBy">
        /// Do you find ORDER BY in the end? </param>
        /// <remarks>
        /// In queries generated for paging, these parts of SqlSelect are stored in two separate locations
        /// condition), this is done in this way,
        /// passed. </remarks>
        private void AppendFromWhereOrderByGroupByHaving(StringBuilder sb, string extraWhere,
            bool includeOrderBy)
        {
            if (from.Length > 0)
            {
                // Write FROM
                sb.Append(SqlKeywords.From);
                // write table list ("A LEFT OUTER JOIN B ON (...) ....")
                sb.Append(from.ToString());
            }

            // if extra filter is specified or if there is an existing filter
            // Will have a WHERE part
            if (extraWhere != null || where != null)
            {
                // Write WHERE
                sb.Append(SqlKeywords.Where);

                // Write the filters prepared in SqlSelect, if any
                if (where != null)
                    sb.Append(@where);

                // If extra filter is specified ...
                if (extraWhere != null)
                {
                    // If sqlSelect has its own filter, AND must be put together
                    if (where != null)
                        sb.Append(" AND ");
                    // Add extra filter
                    sb.Append(extraWhere);
                }
            }

            // Add if grouping
            if (groupBy != null)
            {
                sb.Append(SqlKeywords.GroupBy);
                sb.Append(groupBy);
            }

            // Add if group condition
            if (having != null)
            {
                sb.Append(SqlKeywords.Having);
                sb.Append(having);
            }

            // If there are sorted fields
            if (includeOrderBy && orderBy != null)
            {
                // Write ORDER BY
                sb.Append(SqlKeywords.OrderBy);

                // Add all sorted fields by putting ","
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

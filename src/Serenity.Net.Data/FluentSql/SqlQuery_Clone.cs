namespace Serenity.Data;

public partial class SqlQuery
{
    /// <summary>
    /// Creates a clone of the query.
    /// </summary>
    /// <returns>A cloned query.</returns>
    /// <remarks>
    /// Clones states like TrackAssignments, AssignedFields etc,
    /// creates a copy of Params dictionary
    /// </remarks>
    public SqlQuery Clone()
    {
        var clone = new SqlQuery
        {
            dialect = dialect,
            dialectOverridden = dialectOverridden,
            skip = skip,
            take = take,
            countRecords = countRecords,
            distinct = distinct,
            omitParens = omitParens,
            into = new List<object>(into),
            intoIndex = intoIndex,
            forXml = forXml,
            forJson = forJson,
            unionQuery = unionQuery,
            unionType = unionType,
            nextAutoParam = nextAutoParam
        };

        Column s;
        for (int i = 0; i < columns.Count; i++)
        {
            s = columns[i];
            var si = new Column(s.Expression, s.ColumnName, s.IntoRowIndex, s.IntoField);
            clone.columns.Add(si);
        }

        clone.from = new StringBuilder(from.ToString());
        if (where != null)
            clone.where = new StringBuilder(where.ToString());

        if (orderBy != null)
        {
            clone.orderBy = new List<string>();
            clone.orderBy.AddRange(orderBy);
        }

        if (groupBy != null)
            clone.groupBy = new StringBuilder(groupBy.ToString());

        if (having != null)
            clone.having = new StringBuilder(having.ToString());

        if (parent != null)
            clone.parent = parent;
        else if (Params != null)
            foreach (var pair in Params)
                clone.AddParam(pair.Key, pair.Value);

        if (aliasExpressions != null)
            clone.aliasExpressions = new Dictionary<string, string>(
                aliasExpressions, StringComparer.OrdinalIgnoreCase);

        if (aliasWithJoins != null)
            aliasWithJoins = new Dictionary<string, IHaveJoins>(
                aliasWithJoins, StringComparer.OrdinalIgnoreCase);

        return clone;
    }
}
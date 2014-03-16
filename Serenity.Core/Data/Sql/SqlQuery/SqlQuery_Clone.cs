namespace Serenity.Data
{
    using System.Collections.Generic;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

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
            var clone = new SqlQuery();

            clone.dialect = dialect;
            clone.skip = skip;
            clone.take = take;
            clone.countRecords = countRecords;

            clone.into = new List<IEntity>(into);
            clone.intoIndex = intoIndex;

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

            if (this.Params != null)
                foreach (var pair in this.Params)
                    clone.AddParam(pair.Key, pair.Value);

            return clone;
        }
    }
}
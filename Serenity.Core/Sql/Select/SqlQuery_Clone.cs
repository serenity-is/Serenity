namespace Serenity.Data
{
    using System.Collections.Generic;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    public partial class SqlQuery
    {
        /// <summary>
        ///   Sorgunun birebir kopyasını oluşturur.</summary>
        /// <returns>
        ///   Sorgunun yeni bir kopyası.</returns>
        public SqlQuery Clone()
        {
            SqlQuery clone = new SqlQuery();

            clone.dialect = dialect;
            clone.skip = skip;
            clone.take = take;
            clone.countRecords = countRecords;

            clone.into = new List<Row>(into);
            clone.intoIndex = intoIndex;

            Column s;
            for (int i = 0; i < columns.Count; i++)
            {
                s = columns[i];
                var si = new Column(s.Expression, s.AsAlias, s.IntoRow, s.IntoField);
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

            if (this.parameters != null)
            {
                clone.parameters = new Dictionary();
                foreach (var pair in this.parameters)
                    clone.parameters.Add(pair.Key, pair.Value);
            }

            clone.cachedQuery = cachedQuery;

            return clone;
        }

    }
}
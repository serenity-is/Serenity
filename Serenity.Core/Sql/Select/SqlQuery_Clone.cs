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

            clone._dialect = _dialect;
            clone._skip = _skip;
            clone._take = _take;
            clone._countRecords = _countRecords;

            clone._into = new List<Row>(_into);
            clone._intoIndex = _intoIndex;

            Column s;
            for (int i = 0; i < _columns.Count; i++)
            {
                s = _columns[i];
                var si = new Column(s.Expression, s.AsAlias, s.IntoRow, s.IntoField);
                clone._columns.Add(si);
            }

            clone._from = new StringBuilder(_from.ToString());
            clone._mainTableName = _mainTableName;
            if (_where != null)
                clone._where = new StringBuilder(_where.ToString());
            if (_orderBy != null)
            {
                clone._orderBy = new StringList();
                clone._orderBy.AddRange(_orderBy);
            }
            if (_groupBy != null)
                clone._groupBy = new StringBuilder(_groupBy.ToString());
            if (_having != null)
                clone._having = new StringBuilder(_having.ToString());

            if (this._params != null)
            {
                clone._params = new Dictionary();
                foreach (var pair in this._params)
                    clone._params.Add(pair.Key, pair.Value);
            }

            clone._cachedQuery = _cachedQuery;

            return clone;
        }

    }
}
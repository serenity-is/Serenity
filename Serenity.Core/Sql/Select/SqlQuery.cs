namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;

    public interface IFilterableQuery
    {
        string GetExpression(string fieldName);
        void EnsureForeignJoin(Field field);
    }

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        private int _skip;
        private int _take;
        private bool _distinct;
        private bool _countRecords;
        private List<Row> _into = new List<Row>();
        private int _intoIndex = -1;
        private List<Column> _columns = new List<Column>();
        private StringBuilder _from = new StringBuilder();
        private string _mainTableName;
        private StringBuilder _where;
        private StringList _orderBy;
        private StringBuilder _groupBy;
        private StringBuilder _having;
        private HashSet<string> _joinAliases = null;
        private Dictionary _params;
        private string _cachedQuery;
        private SqlDialect _dialect;

        public SqlQuery()
        {
            _dialect = SqlSettings.CurrentDialect;
        }

        public SqlQuery Into(Row row)
        {
            if (row == null)
                _intoIndex = -1;
            else
            {
                _intoIndex = _into.IndexOf(row);
                if (_intoIndex == -1)
                {
                    _into.Add(row);
                    _intoIndex = _into.Count - 1;
                }
            }
            return this;
        }


        public SqlQuery SubQuery()
        {
            var subQuery = new SqlQuery();
            this.Params = this.Params ?? new Dictionary<string, object>();
            subQuery.Params = this.Params;
            subQuery.parentQuery = this;
            return subQuery;
        }

        public SqlQuery Where(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException(condition);

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, condition);

            EnsureJoinsInCriteria(condition);

            return this;
        }

        void IDbFilterable.Where(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException(condition);

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, condition);

            EnsureJoinsInCriteria(condition);
        }

        public SqlQuery Where(params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                throw new ArgumentNullException("conditions");

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, conditions);

            foreach (var s in conditions)
                EnsureJoinsInCriteria(s);

            return this;
        }

        public SqlQuery OrderBy(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (_orderBy == null)
                _orderBy = new StringList();

            _orderBy.Add(field);

            EnsureJoinsInCriteria(field);

            return this;
        }

        public SqlQuery OrderByOf(int joinAlias, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            OrderBy(joinAlias.TableAliasDot() + field);

            return this;
        }


        public SqlQuery OrderBy(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return OrderBy(field.QueryExpression);
        }

        public SqlQuery OrderBy(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                OrderBy(field);

            return this;
        }

        public SqlQuery OrderByFirstOf(int joinAlias, string field)
        {
            return OrderByFirst(joinAlias.TableAliasDot() + field);
        }

        public SqlQuery OrderByFirst(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (_orderBy == null)
                _orderBy = new StringList();

            //field = PlaceContextLanguage(field);

            if (_orderBy.Contains(field))
                _orderBy.Remove(field);
            else if (field.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
            {
                string s = field.Substring(0, field.Length - 5);
                if (_orderBy.Contains(s))
                    _orderBy.Remove(s);
            }
            else
            {
                string s = field + " DESC";
                if (_orderBy.Contains(s))
                    _orderBy.Remove(s);
            }
          
            if (_orderBy.Count > 0)
                _orderBy.Insert(0, field);
            else
                _orderBy.Add(field);

            EnsureJoinsInCriteria(field);

            return this;
        }

        public SqlQuery OrderByFirst(string field, bool descending)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            if (descending)
                field += Sql.Keyword.Desc;

            return OrderByFirst(field);
        }

        public SqlQuery OrderByDescending(string field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return OrderBy(field + Sql.Keyword.Desc);
        }

        public SqlQuery OrderByDescending(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return OrderByDescending(field.QueryExpression);
        }

        public SqlQuery OrderByDescending(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                OrderByDescending(field);
            return this;
        }

        public SqlQuery OrderBy(params string[] fields)
        {
            foreach (string field in fields)
                OrderBy(field);
            return this;
        }

        public SqlQuery OrderByOf(int joinIndex, params string[] fields)
        {
            foreach (string field in fields)
                OrderByOf(joinIndex, field);
            return this;
        }

        public SqlQuery GroupBy(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref _groupBy, Consts.Comma, field);

            EnsureJoinsInCriteria(field);

            return this;
        }

        public SqlQuery GroupBy(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return GroupBy(field.QueryExpression);
        }

        public SqlQuery GroupBy(params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (string field in fields)
                GroupBy(field);
            return this;
        }

        public SqlQuery GroupBy(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field f in fields)
                GroupBy(f);
            return this;
        }

        public SqlQuery Having(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException("condition");

            _cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref _having, Sql.Keyword.And, condition);
            return this;
        }

        public Row IntoRow
        {
            get { return _into.Count > 0 ? _into[0] : null; }
        }

        public List<Row> IntoRows
        {
            get { return _into; }
        }

        public SqlQuery Skip(int skipRows)
        {
            if (_skip != skipRows)
            {
                _skip = skipRows;
                _cachedQuery = null;
            }
            return this;
        }

        public int Skip()
        {
            return _skip;
        }

        public SqlQuery Take(int rowCount)
        {
            if (_take != rowCount)
            {
                _cachedQuery = null;
                _take = rowCount;
            }
            return this;
        }

        public int Take()
        {
            return _take;
        }

        public SqlQuery Distinct(bool distinct)
        {
            if (_distinct != distinct)
            {
                _cachedQuery = null;
                _distinct = distinct;
            }

            return this;
        }

        public SqlQuery Limit(int skip, int take)
        {
            _cachedQuery = null;
            _skip = skip;
            _take = take;
            return this;
        }

        public string Text
        {
            get { return ToString(); }
        }

        public string MainTableName
        {
            get { return _mainTableName; }
        }

        public void GetFromReader(IDataReader reader)
        {
            GetFromReader(reader, _into);
        }

        public void GetFromReader(IDataReader reader, IList<Row> into)
        {
            int index = 0;
            foreach (var info in _columns)
            {
                if (info.IntoField != null && info.IntoRow != -1)
                {
                    var row = into[info.IntoRow];
                    info.IntoField.GetFromReader(reader, index, row);
                }
                else if (info.IntoRow != -1)
                {
                    var row = into[info.IntoRow];
                    var name = reader.GetName(index);
                    var field = row.FindField(name) ?? row.FindFieldByPropertyName(name);
                    if (field != null)
                    {
                        info.IntoField = field;
                        field.GetFromReader(reader, index, row);
                    }
                    else
                    {
                        if (reader.IsDBNull(index))
                            row.SetDictionaryData(name, null);
                        else
                        {
                            var value = reader.GetValue(index);
                            row.SetDictionaryData(name, value);
                        }                       
                    }
                }

                index++;
            }
        }

        public int GetSelectIntoIndex(Field field)
        {
            return _columns.FindIndex(
                delegate(Column s) { return s.IntoField == field; });
        }

        public string GetExpression(string fieldAlias)
        {
            if (fieldAlias == null || fieldAlias.Length == 0)
                return null;

            Column fieldInfo = _columns.Find(
                delegate(Column s) {
                    return
                        (s.AsAlias != null && s.AsAlias == fieldAlias) ||
                        (String.IsNullOrEmpty(s.AsAlias) && s.Expression == fieldAlias);
                });

            if (fieldInfo == null)
                return null;
            else
            {
                return fieldInfo.Expression ?? fieldInfo.AsAlias;
            }
        }

        public string GetExpression(int selectIndex)
        {
            if (selectIndex < 0 || selectIndex >= _columns.Count)
                throw new ArgumentOutOfRangeException("selectIndex");

            Column si = _columns[selectIndex];
            return si.Expression ?? si.AsAlias;
        }

        public SqlDialect Dialect
        {
            get { return _dialect; }
            set { _dialect = value; _cachedQuery = null; }
        }

        public bool CountRecords
        {
            get { return _countRecords; }
            set { _countRecords = value; _cachedQuery = null; }
        }
    }
}
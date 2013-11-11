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

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery, IDbEnsureJoin
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

        public SqlQuery Select(string expression)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            _cachedQuery = null;
            _columns.Add(new Column(expression, null, _intoIndex, null));
            return this;
        }

        public SqlQuery Select(params string[] expressions)
        {
            foreach (var s in expressions)
                Select(s);
            return this;
        }

        public SqlQuery SelectOf(int joinAlias, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            _cachedQuery = null;
            _columns.Add(new Column(joinAlias.TableAliasDot() + field, field, _intoIndex, null));
            return this;
        }

        public SqlQuery SelectOf(int joinAlias, params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (string field in fields)
                SelectOf(joinAlias, field);
            return this;
        }

        public SqlQuery SelectOf(int joinAlias, params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (var field in fields)
                SelectAs(joinAlias.TableAliasDot() + field.Name, field);

            return this;
        }

        public SqlQuery SelectAs(string expression, string alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");
            if (alias == null || alias.Length == 0)
                throw new ArgumentNullException("alias");

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias, _intoIndex, null));
            return this;
        }

        public SqlQuery SelectAs(string expression, string alias, Action<IDataReader, int> getFromReader)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("field");
            if (alias == null || alias.Length == 0)
                throw new ArgumentNullException("alias");

            var info = new Column(expression, alias, _intoIndex, null);
            info.GetFromReader = getFromReader;
            _cachedQuery = null;
            _columns.Add(info);
            return this;
        }

        public SqlQuery Select(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            if (field.Expression == null)
                _columns.Add(new Column(field.QueryExpression, field.Name, _intoIndex, field));
            else
            {
                EnsureJoinOf(field);
                _columns.Add(new Column(field.Expression, field.Name, _intoIndex, field));
            }

            return this;
        }

        public SqlQuery Select(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                Select(field);
            return this;
        }

        public SqlQuery SelectAs(string expression, Field alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("field");
            if (alias == null)
                throw new ArgumentNullException("alias");

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias.Name, _intoIndex, alias));
            return this;
        }

        void IFilterableQuery.EnsureForeignJoin(Field field)
        {
            this.EnsureJoinOf(field);
        }

        private void EnsureForeignJoin(RowFieldsBase fields, string joinAlias)
        {
            joinAlias = joinAlias.TrimToNull();

            if (joinAlias == null)
                return;

            LeftJoin join;
            if (fields.LeftJoins.TryGetValue(joinAlias, out join) &&
                join.Name != null &&
                !_joinAliases.Contains(join.Name))
            {
                if (join.OnCriteriaAliases != null)
                    foreach (var alias in join.OnCriteriaAliases)
                    {
                        if (alias != null &&
                            String.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) != 0)
                            EnsureForeignJoin(fields, alias);
                    }
                    
                Join(join);
            }
        }

        void IDbEnsureJoin.EnsureJoin(string alias)
        {
            if (this.IntoRow != null)
                EnsureForeignJoin(this.IntoRow.GetFields(), alias);
        }

        public SqlQuery EnsureJoin(LeftJoin join)
        {
            EnsureForeignJoin(join.Fields, join.Name);
            return this;
        }

        public SqlQuery EnsureJoinOf(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (field.ReferencedJoins == null)
                return this;

            foreach (var alias in field.ReferencedJoins)
                EnsureForeignJoin(field.Fields, alias);

            return this;
        }
        
        public SqlQuery From(string table)
        {
            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _from, Consts.Comma, table);
            if (_mainTableName == null)
                _mainTableName = table;
            return this;
        }

        public SqlQuery FromAs(string table, Alias tableAlias)
        {
            return FromAs(table, tableAlias.Name);
        }

        public SqlQuery FromAs(string table, string tableAlias)
        {
            if (tableAlias == null || tableAlias.Length == 0)
                throw new ArgumentNullException("tableAlias");

            From(table);

            _from.Append(' ');
            _from.Append(tableAlias);

            if (_joinAliases == null)
                _joinAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _joinAliases.Add(tableAlias);

            return this;
        }

        public SqlQuery FromAs(Row row, string tableAlias)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            return FromAs(row.Table, tableAlias).Into(row);
        }

        public SqlQuery FromAs(string table, int joinNumber)
        {
            if (joinNumber >= 0)
                return FromAs(table, joinNumber.TableAlias());
            else
                return From(table);
        }

        public SqlQuery FromAs(Row row, int joinNumber)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (joinNumber >= 0)
                return FromAs(row.Table, "T" + joinNumber.ToString()).Into(row);
            else
                return From(row.Table).Into(row);
        }


        public SqlQuery LeftJoin(string joinTable, Alias joinAlias, string joinCondition)
        {
            return LeftJoin(joinTable, joinAlias.Name, joinCondition);
        }

        public SqlQuery LeftJoin(string joinTable, string joinAlias, BaseCriteria joinCondition)
        {
            if (Object.ReferenceEquals(joinCondition, null))
                throw new ArgumentNullException("joinCondition");

            LeftJoin(joinTable, joinAlias, joinCondition.ToString(this));

            return this;
        }

        public SqlQuery LeftJoin(string joinTable, Alias joinAlias, BaseCriteria joinCondition)
        {
            if (Object.ReferenceEquals(joinCondition, null))
                throw new ArgumentNullException("joinCondition");

            LeftJoin(joinTable, joinAlias, joinCondition.ToString(this));

            return this;
        }

        public SqlQuery LeftJoinOn(string thisField, RowFieldsBase joinTable, Alias joinAlias, Field field)
        {
            return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, field) == new Criteria(thisField));
        }

        public SqlQuery LeftJoinOn(Criteria thisField, RowFieldsBase joinTable, Alias joinAlias, Field field)
        {
            return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, field) == thisField);
        }

        public SqlQuery LeftJoinId(string thisField, RowFieldsBase joinTable, Alias joinAlias)
        {
            var idField = joinTable.FirstOrDefault(x => x.Flags.HasFlag(FieldFlags.PrimaryKey));
            if (idField == null)
                throw new ArgumentOutOfRangeException("joinTable", String.Format("Table {0} has no primary key!", joinTable.TableName));

            return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, idField) == new Criteria(thisField));
        }

        public SqlQuery LeftJoinId(RowFieldsBase joinTable, Alias joinAlias, Criteria thisField)
        {
            var idField = joinTable.FirstOrDefault(x => x.Flags.HasFlag(FieldFlags.PrimaryKey));
            if (idField == null)
                throw new ArgumentOutOfRangeException("joinTable", String.Format("Table {0} has no primary key!", joinTable.TableName));

            return LeftJoin(joinTable.TableName, joinAlias, new Criteria(joinAlias, idField) == thisField);
        }

        public SqlQuery LeftJoin(string joinTable, string joinAlias, string joinCondition)
        {
            if (joinTable == null || joinTable.Length == 0)
                throw new ArgumentNullException("joinTable");

            _cachedQuery = null;

            // araya bir boşluk ve LEFT OUTER JOIN metnini ekle.
            AppendUtils.AppendWithSeparator(ref _from, " ", "LEFT OUTER JOIN ");
            // tablo adını ekle
            _from.Append(joinTable);
            // joinAlias belirtilmişse ekle
            if (joinAlias != null && joinAlias.Length > 0)
            {
                _from.Append(' ');
                _from.Append(joinAlias);

                if (_joinAliases == null)
                    _joinAliases = new HashSet<string>();
                _joinAliases.Add(joinAlias);
            }

            // join koşulu belirtilmişse koşulu ON (...) gibi ekle
            if (joinCondition != null && joinCondition.Length > 0)
            {
                _from.Append(" ON (");
                _from.Append(joinCondition);
                _from.Append(')');
            }
            return this;
        }

        public SqlQuery LeftJoin(string joinTable, int joinNumber, BaseCriteria joinCondition)
        {
            if (Object.ReferenceEquals(joinCondition, null))
                throw new ArgumentNullException("joinCondition");

            LeftJoin(joinTable, joinNumber.TableAlias(), joinCondition.ToString(this));

            return this;
        }

        public SqlQuery Join(LeftJoin join)
        {
            return LeftJoin(join.ToTable, join.Name, join.OnCriteria);
        }

        public SqlQuery OuterApply(Alias joinAlias, string expression)
        {
            return OuterApply(joinAlias.Name, expression);
        }

        public SqlQuery SubQuery()
        {
            var subQuery = new SqlQuery();
            this.Params = this.Params ?? new Dictionary<string, object>();
            subQuery.Params = this.Params;
            subQuery.parentQuery = this;
            return subQuery;
        }

        public SqlQuery OuterApply(string joinAlias, string expression)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            if (joinAlias.IsEmptyOrNull())
                throw new ArgumentNullException("joinAlias");

            _cachedQuery = null;

            // araya bir boşluk ve LEFT OUTER JOIN metnini ekle.
            AppendUtils.AppendWithSeparator(ref _from, " ", "OUTER APPLY (");
            // tablo adını ekle
            _from.Append(expression);

            _from.Append(") ");
            _from.Append(joinAlias);

            if (_joinAliases == null)
                _joinAliases = new HashSet<string>();

            _joinAliases.Add(joinAlias);

            return this;
        }

        public SqlQuery Where(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException(condition);

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, condition);
            return this;
        }

        void IDbFilterable.Where(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException(condition);

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, condition);
        }

        public SqlQuery Where(params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                throw new ArgumentNullException("conditions");

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _where, Sql.Keyword.And, conditions);
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

            return this;
        }

        public SqlQuery OrderByOf(int joinAlias, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (_orderBy == null)
                _orderBy = new StringList();

            _orderBy.Add(joinAlias.TableAliasDot() + field);

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
                else if (info.GetFromReader != null)
                    info.GetFromReader(reader, index);
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
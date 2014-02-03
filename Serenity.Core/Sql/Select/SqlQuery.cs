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
        private int skip;
        private int take;
        private bool distinct;
        private bool countRecords;
        private List<Row> into = new List<Row>();
        private int intoIndex = -1;
        private List<Column> columns = new List<Column>();
        private StringBuilder from = new StringBuilder();
        private StringBuilder where;
        private List<string> orderBy;
        private StringBuilder groupBy;
        private StringBuilder having;
        private HashSet<string> joinAliases = null;
        private Dictionary parameters;
        private string cachedQuery;
        private SqlDialect dialect;

        public SqlQuery()
        {
            dialect = SqlSettings.CurrentDialect;
        }

        public SqlQuery Into(Row row)
        {
            if (row == null)
                intoIndex = -1;
            else
            {
                intoIndex = into.IndexOf(row);
                if (intoIndex == -1)
                {
                    into.Add(row);
                    intoIndex = into.Count - 1;
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

            cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref where, Sql.Keyword.And, condition);

            EnsureJoinsInCriteria(condition);

            return this;
        }

        void IDbFilterable.Where(string condition)
        {
            this.Where(condition);
        }

        public SqlQuery Where(params string[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                throw new ArgumentNullException("conditions");

            foreach (var s in conditions)
                Where(conditions);

            return this;
        }

        public SqlQuery OrderBy(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (orderBy == null)
                orderBy = new List<string>();

            orderBy.Add(field);

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

            cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (orderBy == null)
                orderBy = new List<string>();

            //field = PlaceContextLanguage(field);

            if (orderBy.Contains(field))
                orderBy.Remove(field);
            else if (field.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
            {
                string s = field.Substring(0, field.Length - 5);
                if (orderBy.Contains(s))
                    orderBy.Remove(s);
            }
            else
            {
                string s = field + " DESC";
                if (orderBy.Contains(s))
                    orderBy.Remove(s);
            }
          
            if (orderBy.Count > 0)
                orderBy.Insert(0, field);
            else
                orderBy.Add(field);

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

            cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref groupBy, Consts.Comma, field);

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

            cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref having, Sql.Keyword.And, condition);
            return this;
        }

        public Row IntoRow
        {
            get { return into.Count > 0 ? into[0] : null; }
        }

        public List<Row> IntoRows
        {
            get { return into; }
        }

        public SqlQuery Skip(int skipRows)
        {
            if (skip != skipRows)
            {
                skip = skipRows;
                cachedQuery = null;
            }
            return this;
        }

        public int Skip()
        {
            return skip;
        }

        public SqlQuery Take(int rowCount)
        {
            if (take != rowCount)
            {
                cachedQuery = null;
                take = rowCount;
            }
            return this;
        }

        public int Take()
        {
            return take;
        }

        public SqlQuery Distinct(bool distinct)
        {
            if (this.distinct != distinct)
            {
                cachedQuery = null;
                this.distinct = distinct;
            }

            return this;
        }

        public SqlQuery Limit(int skip, int take)
        {
            cachedQuery = null;
            this.skip = skip;
            this.take = take;
            return this;
        }

        public string Text
        {
            get { return ToString(); }
        }

        public void GetFromReader(IDataReader reader)
        {
            GetFromReader(reader, into);
        }

        public void GetFromReader(IDataReader reader, IList<Row> into)
        {
            int index = 0;
            foreach (var info in columns)
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
            return columns.FindIndex(
                delegate(Column s) { return s.IntoField == field; });
        }

        public string GetExpression(string fieldAlias)
        {
            if (fieldAlias == null || fieldAlias.Length == 0)
                return null;

            Column fieldInfo = columns.Find(
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
            if (selectIndex < 0 || selectIndex >= columns.Count)
                throw new ArgumentOutOfRangeException("selectIndex");

            Column si = columns[selectIndex];
            return si.Expression ?? si.AsAlias;
        }

        public SqlDialect Dialect
        {
            get { return dialect; }
            set { dialect = value; cachedQuery = null; }
        }

        public bool CountRecords
        {
            get { return countRecords; }
            set { countRecords = value; cachedQuery = null; }
        }
    }
}
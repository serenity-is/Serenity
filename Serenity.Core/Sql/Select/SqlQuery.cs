namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, IDbGetExpression
    {
        private HashSet<string> aliases;
        private string cachedQuery;
        private List<Column> columns;
        private bool countRecords;
        private SqlDialect dialect;
        private bool distinct;
        private StringBuilder from;
        private StringBuilder having;
        private StringBuilder groupBy;       
        private List<string> orderBy;
        private Dictionary<string, object> parameters;
        private int skip;
        private int take;
        private StringBuilder where;              

        public SqlQuery()
        {
            dialect = SqlSettings.CurrentDialect;
            columns = new List<Column>();
            from = new StringBuilder();
        }

        /// <summary>
        /// Adds a table to the FROM statement. When it is called more than once, puts a comma
        /// between table names (cross join)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(string table)
        {
            if (table.IsEmptyOrNull())
                throw new ArgumentNullException("table");

            cachedQuery = null;

            if (from.Length > 0)
                from.Append(", ");

            from.Append(table);

            return this;
        }

        /// <summary>
        /// Adds a table to the FROM statement with an alias. 
        /// When it is called more than once, puts a comma between table names (cross join)
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="alias">Alias for the table</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(string table, Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (aliases != null &&
                aliases.Contains(alias.Name))
                throw new ArgumentOutOfRangeException("{0} alias is used more than once in the query!");

            From(table);

            from.Append(' ');
            from.Append(alias.Name);

            if (aliases == null)
                aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            aliases.Add(alias.Name);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public SqlQuery From(Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsEmptyOrNull())
                throw new ArgumentNullException("alias.table");

            return From(alias.Table, alias);
        }

        /// <summary>
        /// Adds a field name or a SQL expression to the SELECT statement.
        /// </summary>
        /// <param name="expression">A field or an SQL expression.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No alias is used for the field or expression.</remarks>
        public SqlQuery Select(string expression)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            cachedQuery = null;
            columns.Add(new Column(expression, null, intoIndex, null));
            return this;
        }

        /// <summary>
        /// Adds field names or SQL expressions to the SELECT statement.
        /// </summary>
        /// <param name="expressions">Fields or SQL expressions.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No aliases are used for the fields or expressions.</remarks>
        public SqlQuery Select(params string[] expressions)
        {
            foreach (var s in expressions)
                Select(s);

            return this;
        }

        /// <summary>
        /// Adds a field name or expression to the SELECT statement with a column alias
        /// </summary>
        /// <param name="expression">A field name or SQL expression.</param>
        /// <param name="alias">A column alias.</param>
        /// <returns>The query itself.</returns>
        public SqlQuery SelectAs(string expression, string alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            if (alias == null || alias.Length == 0)
                throw new ArgumentNullException("alias");

            cachedQuery = null;
            columns.Add(new Column(expression, alias, intoIndex, null));

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

        public SqlQuery OrderByFirst(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            cachedQuery = null;

            // sıralama listesi boşsa yeni oluştur
            if (orderBy == null)
                orderBy = new List<string>();

            if (orderBy.Contains(field))
            {
                orderBy.Remove(field);
            }
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

        public SqlQuery OrderBy(params string[] fields)
        {
            foreach (string field in fields)
                OrderBy(field);
            return this;
        }

        public SqlQuery GroupBy(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            cachedQuery = null;

            if (groupBy == null || groupBy.Length == 0)
                groupBy = new StringBuilder(field);
            else
                groupBy.Append(", ").Append(field);

            EnsureJoinsInCriteria(field);

            return this;
        }

        public SqlQuery GroupBy(params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (string field in fields)
                GroupBy(field);
            return this;
        }

        public SqlQuery Having(string condition)
        {
            if (condition == null || condition.Length == 0)
                throw new ArgumentNullException("condition");

            cachedQuery = null;

            if (having == null || having.Length == 0)
                having = new StringBuilder(condition);
            else
                having.Append(Sql.Keyword.And).Append(condition);
            
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

        public SqlQuery Where(string condition)
        {
            if (condition.IsEmptyOrNull())
                throw new ArgumentNullException(condition);

            cachedQuery = null;

            if (where == null || where.Length == 0)
                where = new StringBuilder(condition);
            else
                where.Append(Sql.Keyword.And).Append(condition);

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

        /// <summary>
        ///   SQLSelect'in Select komutları için bir alana karşılık gelen bilgilerin tutulduğu yardımcı 
        ///   sınıf.</summary>
        private class Column
        {
            /// <summary>Alanın adı</summary>
            public string Expression;
            /// <summary>Varsa alana atanan alias</summary>
            public string AsAlias;
            /// <summary>Alan IDataReader'dan hangi Row nesnesinin içine yüklenecek</summary>
            public int IntoRow;
            /// <summary>Alan IDataReader'dan hangi Field nesnesinin içine yüklenecek</summary>
            public Field IntoField;

            public Column(string expression, string asAlias, int intoRow, Field intoField)
            {
                this.Expression = expression;
                this.AsAlias = asAlias;
                this.IntoRow = intoRow;
                this.IntoField = intoField;
            }
        }
    }
}
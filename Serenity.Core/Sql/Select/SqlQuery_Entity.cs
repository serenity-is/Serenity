namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, IDbGetExpression
    {
        private int intoIndex = -1;
        private List<Row> into = new List<Row>();

        /// <summary>
        /// Adds a table to the FROM statement with "T0" alias and sets it as target for future field selections. 
        /// </summary>
        /// <param name="row">Row object.</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(Row row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            return From(row.Table, Alias.T0).Into(row);
        }

        /// <summary>
        /// Adds a field's expression to the SELECT statement with its own column name. 
        /// If a join alias is referenced in the field expression, and the join is defined in 
        /// field's entity class, it is automatically included in the query. 
        /// The field is marked as a target at current index for future loading from a data reader.
        /// </summary>
        /// <param name="field">Field object</param>
        /// <returns>The query itself.</returns>
        public SqlQuery Select(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            cachedQuery = null;

            if (field.Expression == null)
            {
                columns.Add(new Column(field.QueryExpression, field.Name, intoIndex, field));
                return this;
            }

            EnsureJoinOf(field);
            columns.Add(new Column(field.Expression, field.Name, intoIndex, field));

            return this;
        }

        /// <summary>
        /// Adds a field of a given table alias to the SELECT statement.
        /// </summary>
        /// <param name="alias">A table alias that will be prepended to the field name with "." between</param>
        /// <param name="field">A field that only name will be used. It won't be set as a target.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No column name is set for the selected field.
        /// Also field is not set as a target, unlike field only overload, only field name is used.</remarks>
        public SqlQuery Select(Alias alias, Field field)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (field == null)
                throw new ArgumentNullException("field");

            return Select(alias + field);
        }


        /// <summary>
        /// Adds a field of a given table alias to the SELECT statement.
        /// </summary>
        /// <param name="alias">A table alias that will be prepended to the field name with "." between</param>
        /// <param name="field">A field that only its field name will be used. It won't be set as a target.</param>
        /// <param name="columnName">A column name</param>
        /// <returns>The query itself.</returns>
        /// <remarks>Field is not set as a target, unlike field only overload, only field name is used.</remarks>
        public SqlQuery Select(Alias alias, Field field, string columnName)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (field == null)
                throw new ArgumentNullException("field");

            return Select(alias + field, columnName);
        }

        /// <summary>
        /// For each field in the fields array, adds expression of the field to 
        /// the SELECT statement with a column name of its name. 
        /// If a join alias is referenced in the field expression, and the join is defined in 
        /// field's entity class, it is automatically included in the query.
        /// The fields are marked as a target at current index for future loading from a data reader.
        /// </summary>
        /// <param name="fields">Field objects</param>
        /// <returns>The query itself.</returns>
        public SqlQuery Select(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                Select(field);

            return this;
        }

        /// <summary>
        /// Adds a field or an expression to the SELECT statement with a column name of a 
        /// field's name. The field is marked as a target at current index for future loading 
        /// from a data reader.
        /// </summary>
        /// <param name="expression">A field name or an expression</param>
        /// <param name="intoField">A field object whose name to be used as a column name.</param>
        /// <returns>The query itself.</returns>
        public SqlQuery SelectAs(string expression, Field intoField)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("field");

            if (intoField == null)
                throw new ArgumentNullException("alias");

            cachedQuery = null;
            columns.Add(new Column(expression, intoField.Name, intoIndex, intoField));
            return this;
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

        public SqlQuery OrderByDescending(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return OrderBy(field.QueryExpression, desc: true);
        }

        public SqlQuery OrderByDescending(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                OrderByDescending(field);
            return this;
        }

        public SqlQuery GroupBy(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return GroupBy(field.QueryExpression);
        }

        public SqlQuery GroupBy(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field f in fields)
                GroupBy(f);
            return this;
        }

        public int GetSelectIntoIndex(Field field)
        {
            return columns.FindIndex(
                delegate(Column s) { return s.IntoField == field; });
        }

        public Row IntoRow
        {
            get { return into.Count > 0 ? into[0] : null; }
        }

        public List<Row> IntoRows
        {
            get { return into; }
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

        private void EnsureForeignJoin(IDictionary<string, Join> joins, string joinAlias)
        {
            joinAlias = joinAlias.TrimToNull();

            if (joinAlias == null)
                return;

            Join join;
            if (!aliases.Contains(joinAlias) &&
                joins.TryGetValue(joinAlias, out join))
            {
                if (join.ReferencedAliases != null)
                    foreach (var alias in join.ReferencedAliases)
                    {
                        if (alias != null &&
                            String.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) != 0)
                            EnsureForeignJoin(joins, alias);
                    }

                Join(join);
            }
        }

        public SqlQuery EnsureJoin(Join join)
        {
            EnsureForeignJoin(join.Joins, join.Name);
            return this;
        }

        public SqlQuery EnsureJoinOf(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (field.ReferencedJoins == null)
                return this;

            foreach (var alias in field.ReferencedJoins)
                EnsureForeignJoin(field.Fields.Joins, alias);

            return this;
        }

        partial void EnsureJoinsInExpression(string expression)
        {
            if (expression.IsEmptyOrNull())
                return;

            if (this.IntoRow == null)
                return;

            var fields = this.IntoRow.GetFields();

            string referencedJoin;
            var referencedJoins = JoinAliasLocator.LocateOptimized(expression, out referencedJoin);
            if (referencedJoin != null)
                EnsureForeignJoin(fields.Joins, referencedJoin);

            if (referencedJoins != null)
                foreach (var alias in referencedJoins)
                    EnsureForeignJoin(fields.Joins, alias);
        }
    }
}
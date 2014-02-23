namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;

    public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
    {
        private int intoIndex = -1;
        private List<IEntity> into = new List<IEntity>();

        /// <summary>
        /// Adds a table to the FROM statement with "T0" alias and sets it as target for future field selections. 
        /// </summary>
        /// <param name="row">Row object.</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(IEntity row)
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
        public SqlQuery Select(IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            cachedQuery = null;

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
        public SqlQuery Select(Alias alias, IField field)
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
        public SqlQuery Select(Alias alias, IField field, string columnName)
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
        public SqlQuery Select(params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField field in fields)
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
        public SqlQuery SelectAs(string expression, IField intoField)
        {
            if (expression.IsNullOrEmpty())
                throw new ArgumentNullException("field");

            if (intoField == null)
                throw new ArgumentNullException("alias");

            cachedQuery = null;
            columns.Add(new Column(expression, intoField.Name, intoIndex, intoField));
            return this;
        }

        public SqlQuery Into(IEntity row)
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

        public SqlQuery OrderBy(IField field, bool desc = false)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return OrderBy(field.Expression, desc);
        }

        public SqlQuery OrderBy(params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField field in fields)
                OrderBy(field);

            return this;
        }

        public SqlQuery GroupBy(IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return GroupBy(field.Expression);
        }

        public SqlQuery GroupBy(params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField f in fields)
                GroupBy(f);
            return this;
        }

        public int GetSelectIntoIndex(IField field)
        {
            return columns.FindIndex(
                delegate(Column s) { return s.IntoField == field; });
        }

        public IEntity FirstIntoRow
        {
            get { return into.Count > 0 ? into[0] : null; }
        }

        public List<IEntity> IntoRows
        {
            get { return into; }
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

        public SqlQuery EnsureJoinOf(IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            var joinField = field as IFieldWithJoinInfo;
            if (joinField == null)
                return this;

            if (joinField.ReferencedJoins == null)
                return this;

            foreach (var alias in joinField.ReferencedJoins)
                EnsureForeignJoin(joinField.Joins, alias);

            return this;
        }

        partial void EnsureJoinsInExpression(string expression)
        {
            if (expression.IsNullOrEmpty())
                return;

            var intoRow = this.FirstIntoRow as IEntityWithJoins;
            if (intoRow == null)
                return;

            string referencedJoin;
            var referencedJoins = JoinAliasLocator.LocateOptimized(expression, out referencedJoin);
            if (referencedJoin != null)
                EnsureForeignJoin(intoRow.Joins, referencedJoin);

            if (referencedJoins != null)
                foreach (var alias in referencedJoins)
                    EnsureForeignJoin(intoRow.Joins, alias);
        }
    }
}
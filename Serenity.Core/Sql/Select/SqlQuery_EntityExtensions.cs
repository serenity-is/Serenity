namespace Serenity.Data
{
    using System;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
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
        /// Adds a field's expression to the SELECT statement with a column alias of its name. 
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
        /// For each field in the fields array, adds expression of the field to 
        /// the SELECT statement with a column alias of its name. 
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
        /// Adds a field or an expression to the SELECT statement with a column alias of a 
        /// field's name. The field is marked as a target at current index for future loading 
        /// from a data reader.
        /// </summary>
        /// <param name="expression">A field name or an expression</param>
        /// <param name="alias">A field object whose name to be used as an alias.</param>
        /// <returns>The query itself.</returns>
        public SqlQuery SelectAs(string expression, Field alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("field");

            if (alias == null)
                throw new ArgumentNullException("alias");

            cachedQuery = null;
            columns.Add(new Column(expression, alias.Name, intoIndex, alias));
            return this;
        }
    }
}
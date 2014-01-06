namespace Serenity.Data
{
    using System;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
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

            _cachedQuery = null;
            _columns.Add(new Column(expression, null, _intoIndex, null));
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
        /// Adds a field name to the SELECT statement prefixing it with a table alias.
        /// </summary>
        /// <param name="of">A table alias that is inserted before the dot and field name.</param>
        /// <param name="fieldName">A field name.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No alias is used for the field that is added to the SELECT statement.</remarks>
        public SqlQuery Select(Alias of, string fieldName)
        {
            Select(of[fieldName]);

            return this;
        }

        /// <summary>
        /// Adds field names to the SELECT statement prefixing them with a table alias.
        /// </summary>
        /// <param name="of">A table alias that is inserted before the dot and field names.</param>
        /// <param name="fieldNames">Field names.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No aliases are used for the fields that are added to the SELECT statement.</remarks>
        public SqlQuery Select(Alias of, params string[] fieldNames)
        {
            foreach (var s in fieldNames)
                Select(of[s]);

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

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias, _intoIndex, null));

            return this;
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

            _cachedQuery = null;

            if (field.Expression == null)
            {
                _columns.Add(new Column(field.QueryExpression, field.Name, _intoIndex, field));
                return this;
            }

            EnsureJoinOf(field);
            _columns.Add(new Column(field.Expression, field.Name, _intoIndex, field));

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

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias.Name, _intoIndex, alias));
            return this;
        }
    }
}
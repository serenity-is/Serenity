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
    }
}
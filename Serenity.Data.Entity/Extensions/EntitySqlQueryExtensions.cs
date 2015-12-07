using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Extensions for SqlQuery.</summary>
    public static class EntitySqlQueryExtensions
    {
        /// <summary>
        /// Adds a table to the FROM statement with "T0" alias and sets it as target for future field selections. 
        /// </summary>
        /// <param name="row">Row object.</param>
        /// <returns>The query itself.</returns>
        public static SqlQuery From(this SqlQuery query, IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("row");

            var row = entity as Row;
            if (row != null)
            {
                var fields = row.GetFields();
                query.From(fields);
                if (!query.IsDialectOverridden && !string.IsNullOrEmpty(fields.connectionKey))
                {
                    var cs = SqlConnections.TryGetConnectionString(fields.connectionKey);
                    if (cs != null)
                        query.Dialect(cs.Dialect);
                }
            }
            else
            {
                var alias = entity as IAlias;
                if (alias != null && (alias.Name == "t0" || alias.Name == "T0") && alias.Table == entity.Table)
                    query.From(alias);
                else
                    query.From(entity.Table, Alias.T0);
            }
            
            return query.Into(entity);
        }

        public static SqlQuery Into(this SqlQuery query, IEntity into)
        {
            var ext = (ISqlQueryExtensible)query;
            ext.IntoRowSelection(into);

            return query;
        }

        /// <summary>
        /// Adds a field's expression to the SELECT statement with its own column name. 
        /// If a join alias is referenced in the field expression, and the join is defined in 
        /// field's entity class, it is automatically included in the query. 
        /// The field is marked as a target at current index for future loading from a data reader.
        /// </summary>
        /// <param name="field">Field object</param>
        /// <returns>The query itself.</returns>
        public static SqlQuery Select(this SqlQuery query, IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            query.EnsureJoinsInExpression(field.Expression);
            new SqlQuery.Column(query, field.Expression, field.Name, field);
            return query;
        }

        /// <summary>
        /// Adds a field's expression to the SELECT statement with a given column name. 
        /// If a join alias is referenced in the field expression, and the join is defined in 
        /// field's entity class, it is automatically included in the query. 
        /// The field is marked as a target at current index for future loading from a data reader.
        /// </summary>
        /// <param name="field">Field object</param>
        /// <returns>The query itself.</returns>
        public static SqlQuery Select(this SqlQuery query, IField field, string columnName)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            query.EnsureJoinsInExpression(field.Expression);
            new SqlQuery.Column(query, field.Expression, columnName, field);
            return query;
        }

        /// <summary>
        /// Adds a field of a given table alias to the SELECT statement.
        /// </summary>
        /// <param name="alias">A table alias that will be prepended to the field name with "." between</param>
        /// <param name="field">A field that only name will be used. It won't be set as a target.</param>
        /// <returns>The query itself.</returns>
        /// <remarks>No column name is set for the selected field.
        /// Also field is not set as a target, unlike field only overload, only field name is used.</remarks>
        public static SqlQuery Select(this SqlQuery query, IAlias alias, IField field)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (field == null)
                throw new ArgumentNullException("field");

            return query.Select(alias.NameDot + field);
        }


        /// <summary>
        /// Adds a field of a given table alias to the SELECT statement.
        /// </summary>
        /// <param name="alias">A table alias that will be prepended to the field name with "." between</param>
        /// <param name="field">A field that only its field name will be used. It won't be set as a target.</param>
        /// <param name="columnName">A column name</param>
        /// <returns>The query itself.</returns>
        /// <remarks>Field is not set as a target, unlike field only overload, only field name is used.</remarks>
        public static SqlQuery Select(this SqlQuery query, IAlias alias, IField field, string columnName)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (field == null)
                throw new ArgumentNullException("field");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            return query.Select(alias.NameDot + field, columnName);
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
        public static SqlQuery Select(this SqlQuery query, params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField field in fields)
                Select(query, field);

            return query;
        }

        /// <summary>
        /// Adds a field or an expression to the SELECT statement with a column name of a 
        /// field's name. The field is marked as a target at current index for future loading 
        /// from a data reader.
        /// </summary>
        /// <param name="expression">A field name or an expression</param>
        /// <param name="intoField">A field object whose name to be used as a column name.</param>
        /// <returns>The query itself.</returns>
        public static SqlQuery SelectAs(this SqlQuery query, string expression, IField intoField)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("field");

            if (intoField == null)
                throw new ArgumentNullException("alias");

            new SqlQuery.Column(query, expression, intoField.Name, intoField);
            return query;
        }

        public static SqlQuery OrderBy(this SqlQuery query, IField field, bool desc = false)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return query.OrderBy(field.Expression, desc);
        }

        public static SqlQuery OrderBy(this SqlQuery query, params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField field in fields)
                OrderBy(query, field);

            return query;
        }

        public static SqlQuery GroupBy(this SqlQuery query, IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return query.GroupBy(field.Expression);
        }

        public static SqlQuery GroupBy(this SqlQuery query, params IField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (IField f in fields)
                GroupBy(query, f);

            return query;
        }
    }
}
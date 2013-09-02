using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Extensions for objects implementing IDbWhere interface.</summary>
    public static class IDbFilterableExtensions
    {
        /// <summary>
        ///   Adds a filter to query</summary>
        /// <typeparam name="T">
        ///   Query class</typeparam>
        /// <param name="self">
        ///   Query</param>
        /// <param name="filter">
        ///   Filter</param>
        /// <returns>
        ///   Query itself.</returns>
        public static T Where<T>(this T self, BaseCriteria filter) where T: IDbFilterable
        {
            if (!Object.ReferenceEquals(null, filter) && !filter.IsEmpty)
            {
                var statement = filter.ToString(self);
                self.Where(statement);
            }
            return self;
        }

        /// <summary>
        ///   Adds a where statement with equality filter to a query, and sets the parameter value with a parameter.</summary>
        /// <param field="field">
        ///   Field.</param>
        /// <param field="value">
        ///   Parameter value</param>
        /// <returns>
        ///   The new filter parameter.</returns>
        public static T WhereEqual<T>(this T self, Field field, object value) where T : IDbFilterable
        {
            self.Where(new Criteria(field) == self.AddParam(value));
            return self;
        }

        /// <summary>
        ///   Adds all field values in a row to where clause with equality operator and auto named parameters 
        ///   (field name prefixed with '@').</summary>
        /// <param field="row">
        ///   The row with modified field values to be added to the where clause (key row).  Must be in TrackAssignments mode, 
        ///   or an exception is raised.</param>
        /// <returns>
        ///   Object itself.</returns>
        public static T WhereEqual<T>(this T self, Row row) where T : IDbFilterable
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (!row.TrackAssignments)
                throw new ArgumentException("row must be in TrackAssignments mode to determine modified fields.");
            foreach (var field in row.GetFields())
                if (row.IsAssigned(field))
                    self.Where(new Criteria(field) == self.AddParam(field.AsObject(row)));
            return self;
        }

        public static string FilterExpression(this Field field, SqlQuery query)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            string fieldExpr = null;
            if (query != null)
                fieldExpr = query.GetExpression(field.Name);

            if (fieldExpr == null)
            {
                if (field._joinAlias != null)
                    fieldExpr = field._joinAlias + "." + (field._expression ?? field.Name);
                else if (field._expression != null)
                    fieldExpr = field._expression;
                else
                    fieldExpr = (0).TableAliasDot() + field.Name;

                if (query != null)
                    query.EnsureForeignJoin(field);
            }

            return fieldExpr;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public static class SqlRowExtensions
    {
        /// <summary>
        ///   Adds actual table fields in a row to select list of a query.</summary>
        /// <param name="query">
        ///   Query to select fields into (required).</param>
        /// <param name="row">
        ///   Row with fields to be selected (required).</param>
        /// <param name="exclude">
        ///   Fields to be excluded (optional).</param>
        public static SqlQuery SelectTableFields(this SqlQuery query, Row row, params Field[] exclude)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (row == null)
                throw new ArgumentNullException("row");

            HashSet<Field> excludeFields =
                (exclude != null && exclude.Length > 0) ? DbFieldExtensions.ToFieldDictionary(exclude) : null;

            var fields = row.GetFields();

            for (int i = 0; i < row.FieldCount; i++)
            {
                Field field = fields[i];
                if (DbFieldExtensions.IsTableField(field))
                {
                    if (excludeFields == null ||
                        !excludeFields.Contains(field))
                        query.Select(field);
                }
            }

            return query;
        }

        /// <summary>
        ///   Adds foreign / calculated table fields in a row to select list of a query.</summary>
        /// <param name="query">
        ///   Query to select fields into (required).</param>
        /// <param name="row">
        ///   Row with fields to be selected (required).</param>
        /// <param name="exclude">
        ///   Fields to be excluded (optional).</param>
        public static SqlQuery SelectForeignFields(this SqlQuery query, Row row, params Field[] exclude)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (row == null)
                throw new ArgumentNullException("row");

            HashSet<Field> excludeFields =
                (exclude != null && exclude.Length > 0) ? DbFieldExtensions.ToFieldDictionary(exclude) : null;

            var fields = row.GetFields();

            for (int i = 0; i < row.FieldCount; i++)
            {
                Field field = fields[i];
                if (!DbFieldExtensions.IsTableField(field) &&
                    (field.Flags & FieldFlags.ClientSide) != FieldFlags.ClientSide)
                {
                    if (excludeFields == null ||
                        !excludeFields.Contains(field))
                        query.Select(field);
                }
            }

            return query;
        }

        /// <summary>
        ///   Adds foreign / calculated table fields in a row to select list of a query.</summary>
        /// <param name="query">
        ///   Query to select fields into (required).</param>
        /// <param name="row">
        ///   Row with fields to be selected (required).</param>
        /// <param name="exclude">
        ///   Fields to be excluded (optional).</param>
        public static SqlQuery SelectNonTableFields(this SqlQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            foreach (var field in query.IntoRow.GetFields())
            {
                if (!DbFieldExtensions.IsTableField(field) &&
                    (field.Flags & FieldFlags.ClientSide) != FieldFlags.ClientSide)
                {
                    query.Select(field);
                }
            }

            return query;            
        }


        /// <summary>
        ///   Adds actual table fields in a row to select list of a query.</summary>
        /// <param name="query">
        ///   Query to select fields into (required).</param>
        /// <param name="exclude">
        ///   Fields to be excluded (optional).</param>
        public static SqlQuery SelectTableFields(this SqlQuery query, params Field[] exclude)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            return SelectTableFields(query, query.IntoRow, exclude);
        }
        
        public static SqlQuery EnsureAllForeignJoins(this SqlQuery query, Row row)
        {
            foreach (var field in row.GetFields())
                if ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign)
                    query.EnsureJoinOf(field);
            return query;
        }
    }
}

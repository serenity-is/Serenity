using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{

    /// <summary>
    /// Helper for slick grid columns
    /// </summary>
    public static class SlickHelper
    {
        [IncludeGenericArguments(false)]
        public static T SetDefaults<T>(this T columns, string localTextPrefix = null) where T : IEnumerable<SlickColumn>
        {
            foreach (var col in columns)
            {
                col.Sortable = Script.IsValue(col.Sortable) ? col.Sortable : true;
                col.Identifier = col.Identifier ?? col.Field;

                if (localTextPrefix != null &&
                    col.Identifier != null &&
                    (col.Title == null ||
                     col.Title.StartsWith("~")))
                {
                    var key = col.Title != null ? col.Title.Substr(1) : col.Identifier;
                    col.Title = Q.Text(localTextPrefix + key);
                }

                if (col.Formatter == null && col.Format != null)
                    col.Formatter = ConvertToFormatter(col.Format);
                else if (col.Formatter == null)
                    col.Formatter = delegate(int row, int cell, object value, SlickColumn column, dynamic item)
                    {
                        return Q.HtmlEncode(value);
                    };
            }


            return columns;
        }

        public static SlickColumnFormatter ConvertToFormatter(SlickFormatter format)
        {
            if (format == null)
                return null;
            else
                return (row, cell, value, column, item) =>
                {
                    return format(new SlickFormatterContext
                    {
                        Row = row,
                        Cell = cell,
                        Value = value,
                        Column = column,
                        Item = item
                    });
                };
        }
    }
}
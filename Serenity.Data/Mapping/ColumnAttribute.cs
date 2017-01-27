using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this column belongs to another table.
    /// </summary>
    public class ViewColumnAttribute : Attribute
    {
        public ViewColumnAttribute()
        {
        }

        public string Join { get; set; }
        public string Property { get; set; }
    }
}
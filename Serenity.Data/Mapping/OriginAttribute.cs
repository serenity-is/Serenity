using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this column belongs to another table.
    /// </summary>
    public class OriginAttribute : Attribute
    {
        public OriginAttribute(string join, string property = null)
        {
            Check.NotNullOrEmpty(join, nameof(join));
            this.Join = join;
            this.Property = property;
        }

        public string Join { get; private set; }
        public string Property { get; set; }
    }
}
using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this property is not mapped to an SQL column/expression
    /// </summary>
    public class NotMappedAttribute : SetFieldFlagsAttribute
    {
        public NotMappedAttribute()
            : base(FieldFlags.NotMapped)
        {
        }
    }
}
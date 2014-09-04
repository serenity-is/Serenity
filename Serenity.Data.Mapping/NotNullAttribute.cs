using System;

namespace Serenity.Data.Mapping
{
    public class NotNullAttribute : SetFieldFlagsAttribute
    {
        public NotNullAttribute()
            : base(FieldFlags.NotNull)
        {
        }
    }
}
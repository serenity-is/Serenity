using System;

namespace Serenity.Data.Mapping
{
    public class PrimaryKeyAttribute : SetFieldFlagsAttribute
    {
        public PrimaryKeyAttribute()
            : base(FieldFlags.PrimaryKey)
        {
        }
    }
}
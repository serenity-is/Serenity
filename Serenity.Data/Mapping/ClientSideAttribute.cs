using System;

namespace Serenity.Data.Mapping
{
    [Obsolete("Prefer NotMapped attribute")]
    public class ClientSideAttribute : SetFieldFlagsAttribute
    {
        public ClientSideAttribute()
            : base(FieldFlags.ClientSide)
        {
        }
    }
}
using System;

namespace Serenity.Data.Mapping
{
    public class ClientSideAttribute : SetFieldFlagsAttribute
    {
        public ClientSideAttribute()
            : base(FieldFlags.ClientSide)
        {
        }
    }
}
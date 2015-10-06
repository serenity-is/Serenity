using System;

namespace Serenity.Data.Mapping
{
    public class SetFieldFlagsAttribute : Attribute
    {
        public SetFieldFlagsAttribute(FieldFlags add, FieldFlags remove = FieldFlags.None)
        {
            this.Add = add;
            this.Remove = remove;
        }

        public FieldFlags Add { get; private set; }
        public FieldFlags Remove { get; private set; }
    }
}
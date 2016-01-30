using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Used to turn on (include) or turn off (exclude) field flags.
    /// </summary>
    public class SetFieldFlagsAttribute : Attribute
    {
        /// <summary>
        /// Turn on or off field flags.
        /// </summary>
        /// <param name="add">Set of flags to turn on (include)</param>
        /// <param name="remove">Set of flags to turn off (exclude)</param>
        public SetFieldFlagsAttribute(FieldFlags add, FieldFlags remove = FieldFlags.None)
        {
            this.Add = add;
            this.Remove = remove;
        }

        public FieldFlags Add { get; private set; }
        public FieldFlags Remove { get; private set; }
    }
}
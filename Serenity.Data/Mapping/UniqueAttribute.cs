using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueAttribute : SetFieldFlagsAttribute
    {
        public UniqueAttribute()
            : base(FieldFlags.Unique)
        {
            this.CheckBeforeSave = true;
        }

        public string Name { get; set; }
        public bool CheckBeforeSave { get; set; }
        public string ErrorMessage { get; set; }
    }
}
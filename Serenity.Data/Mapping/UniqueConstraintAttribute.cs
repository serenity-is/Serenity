using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UniqueConstraintAttribute : Attribute
    {
        public UniqueConstraintAttribute(params string[] fields)
        {
            if (fields.IsEmptyOrNull())
                throw new ArgumentNullException("fields");

            this.Fields = fields;
            this.CheckBeforeSave = true;
        }

        public string Name { get; set; }
        public string[] Fields { get; private set; }
        public bool CheckBeforeSave { get; set; }
        public bool IgnoreDeleted { get; set; }
        public string ErrorMessage { get; set; }
    }
}
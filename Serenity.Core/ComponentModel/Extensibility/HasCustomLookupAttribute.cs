using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HasCustomLookupAttribute : Attribute
    {
        public HasCustomLookupAttribute(Type customLookupType)
        {
            if (customLookupType == null)
            {
                throw new ArgumentNullException(nameof(customLookupType));
            }
        }
    }
}
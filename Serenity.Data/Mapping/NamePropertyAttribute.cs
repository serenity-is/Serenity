
using System;

namespace Serenity.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NamePropertyAttribute : Attribute
    {
        public NamePropertyAttribute()
        {
        }
    }
}
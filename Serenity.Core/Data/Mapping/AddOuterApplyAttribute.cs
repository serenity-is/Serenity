using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddOuterApplyAttribute : Attribute
    {
        public AddOuterApplyAttribute(string alias, string innerQuery)
        {
            this.Alias = alias;
            this.InnerQuery = innerQuery;
        }

        public String Alias { get; private set; }
        public String InnerQuery { get; private set; }
    }
}
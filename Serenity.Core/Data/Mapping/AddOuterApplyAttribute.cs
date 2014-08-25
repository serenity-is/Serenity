using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OuterApplyAttribute : Attribute
    {
        public OuterApplyAttribute(string alias, string innerQuery)
        {
            this.Alias = alias;
            this.InnerQuery = innerQuery;
        }

        public String Alias { get; private set; }
        public String InnerQuery { get; private set; }
    }

    [Obsolete("Use OuterApplyAttribute instead")]
    public class AddOuterApplyAttribute : OuterApplyAttribute
    {
        public AddOuterApplyAttribute(string alias, string innerQuery)
            : base(alias, innerQuery)
        {
        }
    }
}
using System;

namespace Serenity.Data.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MasterDetailRelationAttribute : Attribute
    {
        public MasterDetailRelationAttribute(string foreignKey)
        {
            this.ForeignKey = foreignKey;
        }

        public string ForeignKey { get; private set; }
    }
}
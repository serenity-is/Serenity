using System;

namespace Serenity.Data.Mapping
{
    public class MinSelectLevelAttribute : Attribute
    {
        public MinSelectLevelAttribute(SelectLevel value)
        {
            this.Value = value;
        }

        public SelectLevel Value { get; private set; }
    }
}
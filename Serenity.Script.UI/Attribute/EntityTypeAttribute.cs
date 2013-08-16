using jQueryApi.UI.Widgets;
using System;

namespace Serenity
{
    public class EntityTypeAttribute : Attribute
    {
        public EntityTypeAttribute(string entityType)
        {
            this.EntityType = entityType;
        }

        public string EntityType  { get; private set; }
    }
}
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public interface IPropertyItemRegistry
    {
        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type);
    }
}
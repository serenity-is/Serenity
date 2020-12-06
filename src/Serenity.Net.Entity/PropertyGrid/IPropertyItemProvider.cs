using Serenity.ComponentModel;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    public interface IPropertyItemProvider
    {
        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type);
    }
}
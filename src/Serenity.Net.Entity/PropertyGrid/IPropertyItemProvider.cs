using Serenity.ComponentModel;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    /// <summary>
    /// IPropertyItemProvider
    /// </summary>
    public interface IPropertyItemProvider
    {
        /// <summary>
        /// Gets the property items for.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type);
    }
}
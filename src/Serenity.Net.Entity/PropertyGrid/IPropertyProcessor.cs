using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    /// <summary>
    /// IPropertyProcessor
    /// </summary>
    public interface IPropertyProcessor
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Processes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="item">The item.</param>
        void Process(IPropertySource source, PropertyItem item);
        /// <summary>
        /// Posts the process.
        /// </summary>
        void PostProcess();
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        List<PropertyItem> Items { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; set; }
        /// <summary>
        /// Gets or sets the based on row.
        /// </summary>
        /// <value>
        /// The based on row.
        /// </value>
        IRow BasedOnRow { get; set; }
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; }
    }
}
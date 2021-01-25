using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity.PropertyGrid
{
    /// <summary>
    /// PropertyProcessor
    /// </summary>
    /// <seealso cref="Serenity.PropertyGrid.IPropertyProcessor" />
    public abstract class PropertyProcessor : IPropertyProcessor
    {
        /// <summary>
        /// Processes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="item">The item.</param>
        public virtual void Process(IPropertySource source, PropertyItem item)
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Posts the process.
        /// </summary>
        public virtual void PostProcess()
        {
        }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public virtual int Priority => 50;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; set; }
        /// <summary>
        /// Gets or sets the based on row.
        /// </summary>
        /// <value>
        /// The based on row.
        /// </value>
        public IRow BasedOnRow { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<PropertyItem> Items { get; set; }
    }
}
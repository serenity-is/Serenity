using System;

namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this column belongs to another table.
    /// </summary>
    public class OriginAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OriginAttribute"/> class.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <param name="property">The property.</param>
        public OriginAttribute(string join, string property = null)
        {
            Join = join ?? throw new ArgumentNullException(nameof(join));
            Property = property;
        }

        /// <summary>
        /// Gets the join.
        /// </summary>
        /// <value>
        /// The join.
        /// </value>
        public string Join { get; private set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; set; }
    }
}
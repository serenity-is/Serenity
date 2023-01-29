namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that this column belongs to another table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OriginAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OriginAttribute"/> class.
        /// </summary>
        /// <param name="join">The join alias.</param>
        public OriginAttribute(string join)
        {
            Join = join ?? throw new ArgumentNullException(nameof(join));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginAttribute"/> class.
        /// </summary>
        /// <param name="join">The join alias.</param>
        /// <param name="property">The property.</param>
        public OriginAttribute(string join, string property)
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
namespace Serenity.Data
{
    /// <summary>
    /// Determines the transaction isolation level used for a controller action
    /// </summary>
    /// <seealso cref="Attribute" />
    public class IsolationLevelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsolationLevelAttribute"/> class.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public IsolationLevelAttribute(IsolationLevel isolationLevel)
        {
            Value = isolationLevel;
        }

        /// <summary>
        /// Gets the isolation level.
        /// </summary>
        /// <value>
        /// The isolation level.
        /// </value>
        public IsolationLevel Value { get; private set; }
    }
}
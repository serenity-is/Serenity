namespace Serenity.PropertyGrid
{
    /// <summary>
    /// Abstraction for the provider that returns a list of property items for a given type
    /// </summary>
    public interface IPropertyItemProvider
    {
        /// <summary>
        /// Gets the property items for.
        /// </summary>
        /// <param name="type">The type.</param>
        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type);
    }
}
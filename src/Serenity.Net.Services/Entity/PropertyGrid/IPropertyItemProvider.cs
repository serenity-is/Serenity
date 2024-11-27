namespace Serenity.PropertyGrid;

/// <summary>
/// Abstraction for the provider that returns a list of property items for a given type
/// </summary>
public interface IPropertyItemProvider
{
    /// <summary>
    /// Gets the property items for specified type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="predicate">Optional predicate that should return true for items to be processed</param>
    public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type, Func<PropertyInfo, bool> predicate = null);
}
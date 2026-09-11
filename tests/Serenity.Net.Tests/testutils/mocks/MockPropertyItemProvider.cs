using Serenity.PropertyGrid;

namespace Serenity.TestUtils;

public class MockPropertyItemProvider : IPropertyItemProvider
{
    public Func<Type, Func<PropertyInfo, bool>?, IEnumerable<PropertyItem>>? Factory { get; set; }

    public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type, Func<PropertyInfo, bool>? predicate = null)
    {
        return Factory?.Invoke(type, predicate) ?? [];
    }
}

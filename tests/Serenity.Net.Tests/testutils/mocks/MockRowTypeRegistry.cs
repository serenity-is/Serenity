namespace Serenity.TestUtils;

public class MockRowTypeRegistry(params Type[] rowTypes) : IRowTypeRegistry
{
    public List<Type> AllRowTypes { get; } = [.. rowTypes ?? []];

    IEnumerable<Type> IRowTypeRegistry.AllRowTypes => AllRowTypes;

    public IEnumerable<Type> ByConnectionKey(string connectionKey)
    {
        return AllRowTypes.Where(t =>
        {
            var attr = t.GetCustomAttribute<ConnectionKeyAttribute>();
            return attr != null && attr.Value == connectionKey;
        });
    }
}
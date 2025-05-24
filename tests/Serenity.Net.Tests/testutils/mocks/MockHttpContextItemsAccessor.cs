namespace Serenity.TestUtils;

public class MockHttpContextItemsAccessor : IHttpContextItemsAccessor
{
    public MockHttpContextItemsAccessor()
    {
        Items = new HashTable<object, object>();
    }

    public IDictionary<object, object> Items { get; private set; }
}

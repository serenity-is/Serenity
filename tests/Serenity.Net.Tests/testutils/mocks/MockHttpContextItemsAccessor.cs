namespace Serenity.TestUtils;

public static partial class TestServiceCollectionExtensions
{
    public class MockHttpContextItemsAccessor : IHttpContextItemsAccessor
    {
        public MockHttpContextItemsAccessor()
        {
            Items = new HashTable<object, object>();
        }

        public IDictionary<object, object> Items { get; private set; }
    }
}
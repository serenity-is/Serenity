namespace Serenity.TestUtils;

public class MockHandlerFactory(Func<Type, Type, object> createHandler = null) : IDefaultHandlerFactory
{
    public Func<Type, Type, object> CreateHandlerCallback = createHandler ?? ((_, _) => throw new NotImplementedException());

    public object CreateHandler(Type rowType, Type handlerInterface)
    {
        return createHandler(rowType, handlerInterface);
    }
}
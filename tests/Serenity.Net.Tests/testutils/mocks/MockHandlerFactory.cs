namespace Serenity.Tests;

public class MockHandlerFactory(Func<Type, Type, object> createHandler) : IDefaultHandlerFactory
{
    private readonly Func<Type, Type, object> createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));

    public object CreateHandler(Type rowType, Type handlerInterface)
    {
        return createHandler(rowType, handlerInterface);
    }
}
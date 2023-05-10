namespace Serenity.Tests;

public class MockHandlerFactory : IDefaultHandlerFactory
{
    private readonly Func<Type, Type, object> createHandler;

    public MockHandlerFactory(Func<Type, Type, object> createHandler)
    {
        this.createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));
    }

    public object CreateHandler(Type rowType, Type handlerInterface)
    {
        return createHandler(rowType, handlerInterface);
    }
}
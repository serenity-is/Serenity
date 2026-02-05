namespace Serenity.TestUtils;

public class MockHandlerFactory(Func<Type, Type, object> createHandler = null) : IDefaultHandlerFactory
{
    public Func<Type, Type, object> CreateHandler { get; set; } = createHandler;

    object IDefaultHandlerFactory.CreateHandler(Type rowType, Type handlerInterface)
    {
        if (CreateHandler is null)
            throw new NotImplementedException("CreateHandler delegate is not set in MockHandlerFactory.");
        
        return CreateHandler(rowType, handlerInterface);
    }
}
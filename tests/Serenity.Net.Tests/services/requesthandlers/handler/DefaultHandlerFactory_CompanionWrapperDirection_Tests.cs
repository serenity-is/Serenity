namespace Serenity.Services;

public class DefaultHandlerFactory_CompanionWrapperDirection_Tests
{
    private class TestRow : Row<TestRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
#pragma warning restore CS0649
        }
    }

    private class CustomDeleteHandlerSync(IRequestContext context) : DeleteRequestHandler<TestRow>(context)
    {
    }

    private class CustomListHandlerSync(IRequestContext context) : ListRequestHandler<TestRow>(context)
    {
    }

    private class CustomRetrieveHandlerSync(IRequestContext context) : RetrieveRequestHandler<TestRow>(context)
    {
    }

    private class CustomUndeleteHandlerSync(IRequestContext context) : UndeleteRequestHandler<TestRow>(context)
    {
    }

    private class CustomDeleteHandlerAsync(IRequestContext context) : DeleteRequestHandlerAsync<TestRow>(context)
    {
    }

    private class CustomListHandlerAsync(IRequestContext context) : ListRequestHandlerAsync<TestRow>(context)
    {
    }

    private class CustomRetrieveHandlerAsync(IRequestContext context) : RetrieveRequestHandlerAsync<TestRow>(context)
    {
    }

    private class CustomUndeleteHandlerAsync(IRequestContext context) : UndeleteRequestHandlerAsync<TestRow>(context)
    {
    }

    private static DefaultHandlerFactory CreateFactory(params Type[] types)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IRequestContext>(new NullRequestContext());
        var provider = services.BuildServiceProvider();
        return new DefaultHandlerFactory(
            new DefaultHandlerRegistry(new MockTypeSource(types)),
            new DefaultHandlerActivator(provider));
    }

    public static TheoryData<Type> ProcessorInterfaces =>
    [
        typeof(ISaveRequestProcessor),
        typeof(ISaveRequestProcessorAsync),
        typeof(IDeleteRequestProcessor),
        typeof(IDeleteRequestProcessorAsync),
        typeof(IListRequestProcessor),
        typeof(IListRequestProcessorAsync),
        typeof(IRetrieveRequestProcessor),
        typeof(IRetrieveRequestProcessorAsync),
        typeof(IUndeleteRequestProcessor),
        typeof(IUndeleteRequestProcessorAsync)
    ];

    [Theory]
    [MemberData(nameof(ProcessorInterfaces))]
    public void CompanionWrapper_Implements_The_Interface_It_Is_Declared_On(Type processorInterface)
    {
        var attr = processorInterface.GetCustomAttribute<CompanionHandlerTypeAttribute>(inherit: true);
        Assert.NotNull(attr);

        var wrapperType = attr.WrapperType.MakeGenericType(typeof(TestRow));

        Assert.True(processorInterface.IsAssignableFrom(wrapperType),
            $"{attr.WrapperType.Name} is declared as the companion wrapper of " +
            $"{processorInterface.Name} but does not implement it.");
    }

    [Theory]
    [MemberData(nameof(ProcessorInterfaces))]
    public void CompanionWrapper_Has_Constructor_Accepting_The_Companion_Interface(Type processorInterface)
    {
        var attr = processorInterface.GetCustomAttribute<CompanionHandlerTypeAttribute>(inherit: true);
        Assert.NotNull(attr);

        var wrapperType = attr.WrapperType.MakeGenericType(typeof(TestRow));

        Assert.NotNull(wrapperType.GetConstructor([attr.CompanionType]));
    }

    [Fact]
    public void Wraps_Sync_Custom_Delete_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomDeleteHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IDeleteRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncDeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomDeleteHandlerSync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Sync_Custom_List_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomListHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IListRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncListRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomListHandlerSync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Sync_Custom_Retrieve_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomRetrieveHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IRetrieveRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncRetrieveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomRetrieveHandlerSync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Sync_Custom_Undelete_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomUndeleteHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IUndeleteRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncUndeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomUndeleteHandlerSync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Async_Custom_Delete_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomDeleteHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IDeleteRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncDeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomDeleteHandlerAsync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Async_Custom_List_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomListHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IListRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncListRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomListHandlerAsync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Async_Custom_Retrieve_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomRetrieveHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IRetrieveRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncRetrieveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomRetrieveHandlerAsync>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Async_Custom_Undelete_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomUndeleteHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IUndeleteRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncUndeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomUndeleteHandlerAsync>(wrapper.WrappedHandler);
    }
}

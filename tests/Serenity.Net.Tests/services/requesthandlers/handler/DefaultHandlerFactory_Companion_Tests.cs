using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

public class DefaultHandlerFactory_Companion_Tests
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

    private class CustomSyncSaveHandler(IRequestContext context) : SaveRequestHandler<TestRow>(context)
    {
    }

    private class CustomAsyncSaveHandler(IRequestContext context) : SaveRequestHandlerAsync<TestRow>(context)
    {
    }

    private class CustomBothSaveHandler(IRequestContext context)
        : SaveRequestHandler<TestRow>(context), ISaveRequestProcessorAsync
    {
        public Task<SaveResponse> ProcessAsync(IUnitOfWork uow, ISaveRequest request, SaveRequestType type,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Process(uow, (SaveRequest<TestRow>)request, type));
        }
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

    [Fact]
    public void Returns_Sync_Custom_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomSyncSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor));

        Assert.IsType<CustomSyncSaveHandler>(handler);
    }

    [Fact]
    public void Returns_Async_Custom_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomAsyncSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessorAsync));

        Assert.IsType<CustomAsyncSaveHandler>(handler);
    }

    [Fact]
    public void Wraps_Sync_Custom_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomSyncSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncSaveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomSyncSaveHandler>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Wraps_Async_Custom_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomAsyncSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncSaveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomAsyncSaveHandler>(wrapper.WrappedHandler);
    }

    [Fact]
    public void Returns_BuiltIn_Sync_When_No_Custom_Handlers()
    {
        var factory = CreateFactory();

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor));

        Assert.IsType<SaveRequestHandler<TestRow>>(handler);
    }

    [Fact]
    public void Returns_BuiltIn_Async_When_No_Custom_Handlers()
    {
        var factory = CreateFactory();

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessorAsync));

        Assert.IsType<SaveRequestHandlerAsync<TestRow>>(handler);
    }

    [Fact]
    public void Returns_Dual_Handler_Without_Wrapping_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomBothSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor));

        Assert.IsType<CustomBothSaveHandler>(handler);
    }

    [Fact]
    public void Returns_Dual_Handler_Without_Wrapping_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomBothSaveHandler));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessorAsync));

        Assert.IsType<CustomBothSaveHandler>(handler);
    }

    private static ServiceProvider CreateProxyProvider(params Type[] types)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IRequestContext>(new NullRequestContext());
        services.AddSingleton<ITypeSource>(new MockTypeSource(types));
        services.AddSingleton<IDefaultHandlerRegistry, DefaultHandlerRegistry>();
        services.AddSingleton<IHandlerActivator, DefaultHandlerActivator>();
        services.AddSingleton<IDefaultHandlerFactory, DefaultHandlerFactory>();
        ServiceCollectionExtensions.AddProxyRequestHandlers(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Async_Create_Proxy_Resolves_Sync_Custom_Handler_When_Only_Sync_Exists()
    {
        using var provider = CreateProxyProvider(typeof(CustomSyncSaveHandler));

        var proxy = provider.GetRequiredService<ICreateHandlerAsync<TestRow>>();

        Assert.IsType<ICreateHandlerAsync<TestRow>>(proxy, exactMatch: false);
    }

    [Fact]
    public void Sync_Create_Proxy_Resolves_Async_Custom_Handler_When_Only_Async_Exists()
    {
        using var provider = CreateProxyProvider(typeof(CustomAsyncSaveHandler));

        var proxy = provider.GetRequiredService<ICreateHandler<TestRow>>();

        Assert.IsType<ICreateHandler<TestRow>>(proxy, exactMatch: false);
    }
}

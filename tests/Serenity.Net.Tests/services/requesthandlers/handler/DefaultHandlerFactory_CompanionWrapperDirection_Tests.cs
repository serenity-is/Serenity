using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services;

/// <summary>
/// Covers the sync/async companion wrapper wiring for Delete / List / Retrieve / Undelete,
/// which had no coverage while Save did (see DefaultHandlerFactory_Companion_Tests).
/// </summary>
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

    /// <summary>
    /// Every processor interface carries a CompanionHandlerTypeAttribute naming the wrapper that
    /// adapts a handler of the other mode. Per that attribute's own documentation the wrapper
    /// adapts a CompanionType handler <i>to the interface the attribute is applied to</i>, so the
    /// wrapper named on an async interface has to implement that async interface, and the wrapper
    /// named on a sync interface has to implement the sync one.
    /// <para>Fails today for the four async interfaces below, because each of them names the
    /// AsyncToSync wrapper that its sync counterpart already names. That wrapper produces the
    /// synchronous interface, so it can never satisfy the async request it is registered for:
    /// IDeleteRequestProcessorAsync, IListRequestProcessorAsync, IRetrieveRequestProcessorAsync
    /// and IUndeleteRequestProcessorAsync. Save is wired correctly and passes.</para>
    /// </summary>
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

    /// <summary>
    /// The second half of the same contract, checked at the constructor. DefaultHandlerFactory
    /// builds the wrapper with Activator.CreateInstance(wrapperType, customHandler), where the
    /// custom handler is the one it found for CompanionType, so the wrapper must expose a
    /// constructor taking exactly that interface or the call cannot bind.
    /// <para>Fails today for the same four async interfaces: they name the AsyncToSync wrapper,
    /// whose constructor takes the async processor, while the handler the factory will hand it is
    /// the synchronous one. This is the direct cause of the MissingMethodException seen in the
    /// four behavioral tests below.</para>
    /// </summary>
    [Theory]
    [MemberData(nameof(ProcessorInterfaces))]
    public void CompanionWrapper_Has_Constructor_Accepting_The_Companion_Interface(Type processorInterface)
    {
        var attr = processorInterface.GetCustomAttribute<CompanionHandlerTypeAttribute>(inherit: true);
        Assert.NotNull(attr);

        var wrapperType = attr.WrapperType.MakeGenericType(typeof(TestRow));

        Assert.NotNull(wrapperType.GetConstructor([attr.CompanionType]));
    }

    /// <summary>
    /// An application has hand-written a synchronous delete handler only, and an async caller asks
    /// the factory for IDeleteRequestProcessorAsync. Since no async custom handler exists, the
    /// factory must not fall back to the built-in generic handler and silently drop the custom
    /// delete rules; it has to return the sync handler adapted by
    /// SyncToAsyncDeleteRequestProcessorWrapper.
    /// <para>Fails today with MissingMethodException from Activator.CreateInstance, because the
    /// wrapper the factory picks is AsyncToSyncDeleteRequestProcessorWrapper and its constructor
    /// will not accept the sync handler. Any upgraded application with a custom sync-only delete
    /// handler hits this on the first async delete request.</para>
    /// </summary>
    [Fact]
    public void Wraps_Sync_Custom_Delete_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomDeleteHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IDeleteRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncDeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomDeleteHandlerSync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// Same scenario for list: only a synchronous custom list handler is registered and an async
    /// caller asks for IListRequestProcessorAsync, so the factory should return it wrapped in
    /// SyncToAsyncListRequestProcessorWrapper and keep the custom filtering and sorting rules.
    /// <para>Fails today with MissingMethodException for the same reason. List is the worst hit of
    /// the four in practice, since every grid load goes through it.</para>
    /// </summary>
    [Fact]
    public void Wraps_Sync_Custom_List_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomListHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IListRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncListRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomListHandlerSync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// Same scenario for retrieve: a synchronous custom retrieve handler exists and an async caller
    /// asks for IRetrieveRequestProcessorAsync, which should yield
    /// SyncToAsyncRetrieveRequestProcessorWrapper around the custom handler.
    /// <para>Fails today with MissingMethodException, so opening an edit dialog for such a row
    /// throws instead of returning the record.</para>
    /// </summary>
    [Fact]
    public void Wraps_Sync_Custom_Retrieve_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomRetrieveHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IRetrieveRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncRetrieveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomRetrieveHandlerSync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// Same scenario for undelete: a synchronous custom undelete handler exists and an async caller
    /// asks for IUndeleteRequestProcessorAsync, which should yield
    /// SyncToAsyncUndeleteRequestProcessorWrapper around the custom handler.
    /// <para>Fails today with MissingMethodException. Undelete only applies to rows with an
    /// IsActiveDeletedRow or similar marker, so fewer applications reach it, but the wiring defect
    /// is identical.</para>
    /// </summary>
    [Fact]
    public void Wraps_Sync_Custom_Undelete_When_Requesting_Async()
    {
        var factory = CreateFactory(typeof(CustomUndeleteHandlerSync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IUndeleteRequestProcessorAsync));

        var wrapper = Assert.IsType<SyncToAsyncUndeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomUndeleteHandlerSync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// The mirror of the delete case, and the direction that works today: only an async custom
    /// delete handler exists while a synchronous caller asks for IDeleteRequestProcessor, so the
    /// factory returns it inside AsyncToSyncDeleteRequestProcessorWrapper, which blocks on the task.
    /// <para>Passes on the current wiring. It is here so that correcting the async attribute cannot
    /// be done by editing the sync one instead, which would move the failure rather than fix it.</para>
    /// </summary>
    [Fact]
    public void Wraps_Async_Custom_Delete_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomDeleteHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IDeleteRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncDeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomDeleteHandlerAsync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// The working direction for list: an async-only custom list handler is adapted to
    /// IListRequestProcessor through AsyncToSyncListRequestProcessorWrapper. Passes today, and
    /// guards the sync attribute against a misplaced fix.
    /// </summary>
    [Fact]
    public void Wraps_Async_Custom_List_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomListHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IListRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncListRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomListHandlerAsync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// The working direction for retrieve: an async-only custom retrieve handler is adapted to
    /// IRetrieveRequestProcessor through AsyncToSyncRetrieveRequestProcessorWrapper. Passes today,
    /// and guards the sync attribute against a misplaced fix.
    /// </summary>
    [Fact]
    public void Wraps_Async_Custom_Retrieve_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomRetrieveHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IRetrieveRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncRetrieveRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomRetrieveHandlerAsync>(wrapper.WrappedHandler);
    }

    /// <summary>
    /// The working direction for undelete: an async-only custom undelete handler is adapted to
    /// IUndeleteRequestProcessor through AsyncToSyncUndeleteRequestProcessorWrapper. Passes today,
    /// and guards the sync attribute against a misplaced fix.
    /// </summary>
    [Fact]
    public void Wraps_Async_Custom_Undelete_When_Requesting_Sync()
    {
        var factory = CreateFactory(typeof(CustomUndeleteHandlerAsync));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(IUndeleteRequestProcessor));

        var wrapper = Assert.IsType<AsyncToSyncUndeleteRequestProcessorWrapper<TestRow>>(handler);
        Assert.IsType<CustomUndeleteHandlerAsync>(wrapper.WrappedHandler);
    }
}

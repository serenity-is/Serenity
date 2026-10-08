namespace Serenity.Services;

public class DefaultHandlerFactoryTests
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class TestRow : Row<TestRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty, Identity]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty, Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
        }
    }

    private class HandlerA(IRequestContext context) : SaveRequestHandler<TestRow>(context)
    {
    }

    [DefaultHandler]
    private class HandlerB(IRequestContext context) : SaveRequestHandler<TestRow>(context)
    {
    }

    private class HandlerC(IRequestContext context) : SaveRequestHandler<TestRow>(context)
    {
    }

    [DefaultHandler]
    private class HandlerD(IRequestContext context) : SaveRequestHandler<TestRow>(context)
    {
    }

    private static DefaultHandlerFactory CreateFactory(params Type[] types)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IRequestContext>(new NullRequestContext());
        var provider = services.BuildServiceProvider();
        return new DefaultHandlerFactory(
            new DefaultHandlerRegistry(new MockTypeSource(types)),
            new DefaultHandlerActivator(new DefaultServiceProviderAccessor(provider)));
    }

    [Fact]
    public void Constructor_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHandlerFactory(null, new DefaultHandlerActivator(new DefaultServiceProviderAccessor(new ServiceCollection().BuildServiceProvider()))));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHandlerFactory(new DefaultHandlerRegistry(new MockTypeSource()), null));
    }

    [Fact]
    public void CreateHandler_Throws_For_Null_RowType()
    {
        var factory = CreateFactory();
        Assert.Throws<ArgumentNullException>(() => factory.CreateHandler(null, typeof(ISaveRequestProcessor)));
    }

    [Fact]
    public void CreateHandler_Throws_For_Null_HandlerInterface()
    {
        var factory = CreateFactory();
        Assert.Throws<ArgumentNullException>(() => factory.CreateHandler(typeof(TestRow), null));
    }

    [Fact]
    public void Returns_Default_When_Multiple_Handlers_And_One_Is_Default()
    {
        var factory = CreateFactory(typeof(HandlerA), typeof(HandlerB));

        var handler = factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor));

        Assert.IsType<HandlerB>(handler);
    }

    [Fact]
    public void Throws_When_Multiple_Handlers_And_No_Default()
    {
        var factory = CreateFactory(typeof(HandlerA), typeof(HandlerC));

        Assert.Throws<InvalidProgramException>(() =>
            factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor)));
    }

    [Fact]
    public void Throws_When_Multiple_Handlers_And_Multiple_Defaults()
    {
        var factory = CreateFactory(typeof(HandlerB), typeof(HandlerD));

        Assert.Throws<InvalidProgramException>(() =>
            factory.CreateHandler(typeof(TestRow), typeof(ISaveRequestProcessor)));
    }
}

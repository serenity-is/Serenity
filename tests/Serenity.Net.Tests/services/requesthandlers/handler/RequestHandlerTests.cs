namespace Serenity.Services;

public class RequestHandlerTests
{
    private class TestRow : Row<TestRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty, Identity]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty, Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    private class PlainHandler : IRequestHandler
    {
    }

    private class TestBaseRequestHandler(IRequestContext context) : BaseRequestHandler(context)
    {
        public IRequestContext ContextPublic => Context;
        public ITwoLevelCache CachePublic => Cache;
        public ITextLocalizer LocalizerPublic => Localizer;
        public IPermissionService PermissionsPublic => Permissions;
        public ClaimsPrincipal? UserPublic => User;
    }

    public class RequestHandlerExtensionsTests
    {
        [Fact]
        public void CreateRequest_List_Creates_ListRequest()
        {
            var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
            Assert.IsType<ListRequest>(handler.CreateRequest());
        }

        [Fact]
        public void CreateRequest_Retrieve_Creates_RetrieveRequest()
        {
            var handler = new RetrieveRequestHandler<TestRow>(new NullRequestContext());
            Assert.IsType<RetrieveRequest>(handler.CreateRequest());
        }

        [Fact]
        public void CreateRequest_Delete_Creates_DeleteRequest()
        {
            var handler = new DeleteRequestHandler<TestRow>(new NullRequestContext());
            Assert.IsType<DeleteRequest>(handler.CreateRequest());
        }

        [Fact]
        public void CreateRequest_Undelete_Creates_UndeleteRequest()
        {
            var handler = new UndeleteRequestHandler<TestRow>(new NullRequestContext());
            Assert.IsType<UndeleteRequest>(handler.CreateRequest());
        }

        [Fact]
        public void CreateRequest_Save_Generic_Creates_SaveRequest()
        {
            var handler = new SaveRequestHandler<TestRow>(new NullRequestContext());
            var request = handler.CreateRequest<TestRow>();
            Assert.IsType<SaveRequest<TestRow>>(request);
        }

        [Fact]
        public void CreateRequest_Save_NonGeneric_Creates_SaveRequest()
        {
            var handler = new SaveRequestHandler<TestRow>(new NullRequestContext());
            ISaveRequest request = handler.CreateRequest();
            Assert.IsType<SaveRequest<TestRow>>(request);
        }

        [Fact]
        public void GetRequestType_Returns_Type_For_List_Handler()
        {
            var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
            Assert.Equal(typeof(ListRequest), handler.GetRequestType());
        }

        [Fact]
        public void GetRequestType_Returns_Null_When_Not_Declared()
        {
            Assert.Null(new PlainHandler().GetRequestType());
        }

        [Fact]
        public void GetResponseType_Returns_Type_For_List_Handler()
        {
            var handler = new ListRequestHandler<TestRow>(new NullRequestContext());
            Assert.Equal(typeof(ListResponse<TestRow>), handler.GetResponseType());
        }

        [Fact]
        public void GetResponseType_Returns_Null_When_Not_Declared()
        {
            Assert.Null(new PlainHandler().GetResponseType());
        }

        [Fact]
        public void GetRequestType_Throws_For_Null()
        {
            Assert.Throws<ArgumentNullException>(() => RequestHandlerExtensions.GetRequestType(null));
        }

        [Fact]
        public void GetResponseType_Throws_For_Null()
        {
            Assert.Throws<ArgumentNullException>(() => RequestHandlerExtensions.GetResponseType(null));
        }
    }

    public class BaseRequestHandlerTests
    {
        [Fact]
        public void Constructor_Throws_For_Null_Context()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBaseRequestHandler(null));
        }

        [Fact]
        public void Exposes_Context_Services()
        {
            var context = new NullRequestContext();
            var handler = new TestBaseRequestHandler(context);

            Assert.Same(context, handler.ContextPublic);
            Assert.Same(context.Cache, handler.CachePublic);
            Assert.Same(context.Localizer, handler.LocalizerPublic);
            Assert.Same(context.Permissions, handler.PermissionsPublic);
            Assert.Null(handler.UserPublic);
        }
    }

    public class DefaultHandlerFactoryTests
    {
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
                new DefaultHandlerActivator(provider));
        }

        [Fact]
        public void Constructor_Throws_For_Nulls()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultHandlerFactory(null, new DefaultHandlerActivator(new ServiceCollection().BuildServiceProvider())));
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
}

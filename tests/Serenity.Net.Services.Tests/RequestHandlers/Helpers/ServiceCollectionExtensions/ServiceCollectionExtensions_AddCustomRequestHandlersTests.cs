namespace Serenity.Tests.Services;

public class ServiceCollectionExtensions_AddCustomRequestHandlersTests
{
    [Fact]
    public void Throws_ArgumentNull_When_Collection_DoesNotHave_ITypeSource()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var collection = new ServiceCollection();
            collection.AddCustomRequestHandlers();
        });
    }

    [Fact]
    public void Does_Not_Throw_If_TypeSource_Is_Added_To_Collection()
    {
        var typeSource = new DefaultTypeSource(Array.Empty<Assembly>());
        var collection = new ServiceCollection();
        collection.AddSingleton<ITypeSource>(typeSource);
        Assert.Single(collection);
        collection.AddCustomRequestHandlers();
        Assert.Single(collection);
    }

    [Fact]
    public void Uses_Passed_TypeSource()
    {
        var typeSource = new MockTypeSource(typeof(CustomHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection, 
            x1 =>
            {
                Assert.Equal(typeof(CustomHandler1), x1.ServiceType);
                Assert.Equal(typeof(CustomHandler1), x1.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x1.Lifetime);
            }, 
            x2 =>
            {
                Assert.Equal(typeof(ICustomHandler1), x2.ServiceType);
                Assert.Equal(typeof(CustomHandler1), x2.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x2.Lifetime);
            });
    }

    [Fact]
    public void Concrete_Type_CanBeFiltered_Via_Predicate()
    {
        var typeSource = new MockTypeSource(typeof(CustomHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource, (svc, impl) =>
            svc.IsInterface);
        var descriptor = Assert.Single(collection);
        Assert.Equal(typeof(ICustomHandler1), descriptor.ServiceType);
        Assert.Equal(typeof(CustomHandler1), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void Interface_Type_CanBeFiltered_Via_Predicate()
    {
        var typeSource = new MockTypeSource(typeof(CustomHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource, (svc, impl) =>
            !svc.IsInterface);
        var descriptor = Assert.Single(collection);
        Assert.Equal(typeof(CustomHandler1), descriptor.ServiceType);
        Assert.Equal(typeof(CustomHandler1), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void Abstract_Types_Are_Ignored()
    {
        var typeSource = new MockTypeSource(typeof(AbstractHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Empty(collection);
    }

    [Fact]
    public void Interface_Types_Are_Ignored()
    {
        var typeSource = new MockTypeSource(typeof(IInterfaceHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Empty(collection);
    }

    [Fact]
    public void Open_Generic_Types_Are_Ignored()
    {
        var typeSource = new MockTypeSource(
            typeof(GenericHandler1<>),
            typeof(GenericHandler2<,>));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Empty(collection);
    }

    [Fact]
    public void Interfaces_That_Does_Not_Implement_IRequestHandler_Are_Ignored()
    {
        var typeSource = new MockTypeSource(
            typeof(NotRequestHandler));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Empty(collection);
    }

    [Fact]
    public void Closed_Generic_Types_Are_NOTIgnored()
    {
        var typeSource = new MockTypeSource(
            typeof(GenericDerivedHandler1),
            typeof(GenericDerivedHandler2));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            x1 =>
            {
                Assert.Equal(typeof(GenericDerivedHandler1), x1.ServiceType);
                Assert.Equal(typeof(GenericDerivedHandler1), x1.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x1.Lifetime);
            },
            x2 =>
            {
                Assert.Equal(typeof(GenericDerivedHandler2), x2.ServiceType);
                Assert.Equal(typeof(GenericDerivedHandler2), x2.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x2.Lifetime);
            },
            x3 =>
            {
                Assert.Equal(typeof(ICustomHandler1), x3.ServiceType);
                Assert.Equal(typeof(GenericDerivedHandler1), x3.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x3.Lifetime);
            },
            x4 =>
            {
                Assert.Equal(typeof(ICustomHandler2), x4.ServiceType);
                Assert.Equal(typeof(GenericDerivedHandler2), x4.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x4.Lifetime);
            });
    }

    [Fact]
    public void SaveRequestHandler_Correct_Interfaces_Are_Generated()
    {
        var typeSource = new MockTypeSource(
            typeof(TestSaveHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            x => Assert.Equal(typeof(ICreateHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(ICreateHandler<TestRow, SaveRequest<TestRow>, SaveResponse>), x.ServiceType),
            x => Assert.Equal(typeof(ISaveHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(ISaveHandler<TestRow, SaveRequest<TestRow>, SaveResponse>), x.ServiceType),
            x => Assert.Equal(typeof(IUpdateHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(IUpdateHandler<TestRow, SaveRequest<TestRow>, SaveResponse>), x.ServiceType),
            x => Assert.Equal(typeof(TestSaveHandler1), x.ServiceType));
    }

    [Fact]
    public void DeleteRequestHandler_Correct_Interfaces_Are_Generated()
    {
        var typeSource = new MockTypeSource(
            typeof(TestDeleteHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            x => Assert.Equal(typeof(IDeleteHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(IDeleteHandler<TestRow, DeleteRequest, DeleteResponse>), x.ServiceType),
            x => Assert.Equal(typeof(TestDeleteHandler1), x.ServiceType));
    }

    [Fact]
    public void RetrieveRequestHandler_Correct_Interfaces_Are_Generated()
    {
        var typeSource = new MockTypeSource(
            typeof(TestRetrieveHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            x => Assert.Equal(typeof(IRetrieveHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(IRetrieveHandler<TestRow, RetrieveRequest, RetrieveResponse<TestRow>>), x.ServiceType),
            x => Assert.Equal(typeof(TestRetrieveHandler1), x.ServiceType));
    }

    [Fact]
    public void ListRequestHandler_Correct_Interfaces_Are_Generated()
    {
        var typeSource = new MockTypeSource(
            typeof(TestListHandler1));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            x => Assert.Equal(typeof(IListHandler<TestRow>), x.ServiceType),
            x => Assert.Equal(typeof(IListHandler<TestRow, ListRequest>), x.ServiceType),
            x => Assert.Equal(typeof(IListHandler<TestRow, ListRequest, ListResponse<TestRow>>), x.ServiceType),
            x => Assert.Equal(typeof(TestListHandler1), x.ServiceType));
    }

    [Fact]
    public void Throws_When_Multiple_Types_Without_DefaultHandler_Implements_Same_Interface()
    {
        var typeSource = new MockTypeSource(
            typeof(CustomHandler1A),
            typeof(CustomHandler1B));
        var collection = new ServiceCollection();
        var exception = Assert.Throws<InvalidProgramException>(() => 
            collection.AddCustomRequestHandlers(typeSource));
        Assert.Contains("CustomHandler1A", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CustomHandler1B", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add [DefaultHandler] to one of them!", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Throws_When_Multiple_Types_With_DefaultHandler_Implements_Same_Interface()
    {
        var typeSource = new MockTypeSource(
            typeof(CustomHandler1DefaultA),
            typeof(CustomHandler1DefaultB));
        var collection = new ServiceCollection();
        var exception = Assert.Throws<InvalidProgramException>(() =>
            collection.AddCustomRequestHandlers(typeSource));
        Assert.Contains("CustomHandler1DefaultA", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CustomHandler1DefaultB", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("use [DefaultHandler] on only one of them!", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Uses_One_With_DefaultHandler_If_Multiple_Types_Implement_Same_Interface()
    {
        var typeSource = new MockTypeSource(
            typeof(CustomHandler1A),
            typeof(CustomHandler1DefaultA));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        Assert.Collection(collection.OrderBy(x => x.ServiceType?.Name),
            (x) =>
            {
                Assert.Equal(typeof(CustomHandler1A), x.ServiceType);
                Assert.Equal(typeof(CustomHandler1A), x.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x.Lifetime);
            },
            (x) => 
            {
                Assert.Equal(typeof(CustomHandler1DefaultA), x.ServiceType);
                Assert.Equal(typeof(CustomHandler1DefaultA), x.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x.Lifetime);
            },
            (x) =>
            {
                Assert.Equal(typeof(ICustomHandler1), x.ServiceType);
                Assert.Equal(typeof(CustomHandler1DefaultA), x.ImplementationType);
                Assert.Equal(ServiceLifetime.Transient, x.Lifetime);
            });
    }

    private interface INotRequestHandler
    {
    }

    private class NotRequestHandler : INotRequestHandler
    {
    }

    private interface ICustomHandler1 : IRequestHandler
    {
    }

    private interface ICustomHandler2 : IRequestHandler
    {
    }

    private class CustomHandler1 : ICustomHandler1
    {
    }

    [DefaultHandler]
    private class CustomHandler1DefaultA : ICustomHandler1
    {
    }

    [DefaultHandler]
    private class CustomHandler1DefaultB : ICustomHandler1
    {
    }

    private class CustomHandler1A : ICustomHandler1
    {
    }

    private class CustomHandler1B : ICustomHandler1
    {
    }

    private abstract class AbstractHandler1 : ICustomHandler1
    {
    }

    private class GenericHandler1<T1> : ICustomHandler1
    {
    }

    private class GenericHandler2<T1, T2> : ICustomHandler2
    {
    }

    private class GenericDerivedHandler1 : GenericHandler1<int>
    {
    }

    private class GenericDerivedHandler2 : GenericHandler2<int, double>
    {
    }

    private interface IInterfaceHandler1 : ICustomHandler1
    {
    }

    private class TestSaveHandler1 : SaveRequestHandler<TestRow>
    {
        public TestSaveHandler1(IRequestContext context) : base(context)
        {
        }
    }

    private class TestListHandler1 : ListRequestHandler<TestRow>
    {
        public TestListHandler1(IRequestContext context) : base(context)
        {
        }
    }

    private class TestRetrieveHandler1 : RetrieveRequestHandler<TestRow>
    {
        public TestRetrieveHandler1(IRequestContext context) : base(context)
        {
        }
    }

    private class TestDeleteHandler1 : DeleteRequestHandler<TestRow>
    {
        public TestDeleteHandler1(IRequestContext context) : base(context)
        {
        }
    }

    public class TestRow : Row<TestRow.RowFields>, IIdRow
    {
        [IdProperty]
        public Guid? ID
        {
            get { return fields.ID[this]; }
            set { fields.ID[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public GuidField ID;
        }
    }

}
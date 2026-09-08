using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Serenity.Reflection;

namespace Serenity.Extensions.DependencyInjection;

public class CoreServiceCollectionExtensionsTests
{
    [Fact]
    public void RegisterSingleton_RegistersTheInterface_AsASingleton()
    {
        var collection = new ServiceCollection();
        var typeSource = new MockTypeSource(typeof(TestRegSingleton1));
        collection.AddAutoRegisteredServices(typeSource);
        var services = collection.BuildServiceProvider();
        var service = services.GetService<ITestService1>();
        Assert.NotNull(service);
        Assert.IsType<TestRegSingleton1>(service);
    }

    [Fact]
    public void RegisterSingleton_Does_Not_Reqister_ConcreteType_Without_AsSelf()
    {
        var collection = new ServiceCollection();
        var typeSource = new MockTypeSource(typeof(TestRegSingleton1));
        collection.AddAutoRegisteredServices(typeSource);
        var services = collection.BuildServiceProvider();
        var service = services.GetService<TestRegSingleton1>();
        Assert.Null(service);
    }

    [Fact]
    public void RegisterSingleton_Adds_ConcreteType_With_Self()
    {
        var collection = new ServiceCollection();
        var typeSource = new MockTypeSource(typeof(TestRegSingletonAsSelf1));
        collection.AddAutoRegisteredServices(typeSource);
        var services = collection.BuildServiceProvider();
        var service = services.GetService<TestRegSingletonAsSelf1>();
        Assert.IsType<TestRegSingletonAsSelf1>(service);
        var intf = services.GetService<ITestService1>();
        Assert.IsType<TestRegSingletonAsSelf1>(intf);
    }

    [Fact]
    public void RegisterSingleton_With_Empty_Types_Skips_Interfaces()
    {
        var collection = new ServiceCollection();
        var typeSource = new MockTypeSource(typeof(TestRegSingletonAsSelfEmptyTypes));
        collection.AddAutoRegisteredServices(typeSource);
        var services = collection.BuildServiceProvider();
        var service = services.GetService<TestRegSingletonAsSelfEmptyTypes>();
        Assert.IsType<TestRegSingletonAsSelfEmptyTypes>(service);
        var intf = services.GetService<ITestService1>();
        Assert.Null(intf);
    }


    interface ITestService1
    {
    }

    [RegisterSingleton]
    class TestRegSingleton1 : ITestService1
    {
    }

    [RegisterSingleton(ReplaceExisting = true)]
    class TestRegReplaceExisting : ITestService1
    {
    }


    [RegisterSingleton(AsSelf = true)]
    class TestRegSingletonAsSelf1 : ITestService1
    {
    }

    [RegisterSingleton([], AsSelf = true)]
    class TestRegSingletonAsSelfEmptyTypes : ITestService1
    {
    }

    [Fact]
    public void RegisterSingleton_AsSelf_NoInterfaces_Throws()
    {
        var collection = new ServiceCollection();
        Assert.Throws<InvalidProgramException>(() =>
            collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonAsSelfNoInterface))));
    }

    [Fact]
    public void RegisterSingleton_AsSelf_MultipleInterfaces_NoNameMatch_Throws()
    {
        var collection = new ServiceCollection();
        Assert.Throws<InvalidProgramException>(() =>
            collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonAsSelfMultipleIntfs))));
    }

    [RegisterSingleton(AsSelf = true)]
    class TestRegSingletonAsSelfNoInterface { }

    [RegisterSingleton(AsSelf = true)]
    class TestRegSingletonAsSelfMultipleIntfs : ITestService1, ITestService2 { }

    [Fact]
    public void RegisterScoped_RegistersTheInterface_AsScoped()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegScoped1)));
        var provider = collection.BuildServiceProvider();

        ITestService1 inScope1A, inScope1B, inScope2;
        using (var scope1 = provider.CreateScope())
        {
            inScope1A = scope1.ServiceProvider.GetService<ITestService1>();
            inScope1B = scope1.ServiceProvider.GetService<ITestService1>();
        }
        using (var scope2 = provider.CreateScope())
            inScope2 = scope2.ServiceProvider.GetService<ITestService1>();

        Assert.IsType<TestRegScoped1>(inScope1A);
        Assert.Same(inScope1A, inScope1B);
        Assert.NotSame(inScope1A, inScope2);
    }

    [Fact]
    public void RegisterTransient_RegistersTheInterface_AsTransient()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegTransient1)));
        var provider = collection.BuildServiceProvider();

        var instance1 = provider.GetService<ITestService1>();
        var instance2 = provider.GetService<ITestService1>();

        Assert.IsType<TestRegTransient1>(instance1);
        Assert.NotSame(instance1, instance2);
    }

    [RegisterScoped]
    class TestRegScoped1 : ITestService1 { }

    [RegisterTransient]
    class TestRegTransient1 : ITestService1 { }

    [Fact]
    public void RegisterSingleton_SkipExisting_True_UserPreregistrationWins()
    {
        var collection = new ServiceCollection();
        var userImpl = new TestRegSingleton_UserImpl();
        collection.AddSingleton<ITestService1>(userImpl);
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingleton1)));

        Assert.Same(userImpl, collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void RegisterSingleton_SkipExisting_False_AlwaysAdds_AllRegistrationsPresent()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(
            typeof(TestRegSingletonAlwaysAdd1), typeof(TestRegSingletonAlwaysAdd2)));
        var provider = collection.BuildServiceProvider();

        var services = provider.GetServices<ITestService2>().ToList();
        Assert.Equal(2, services.Count);
        Assert.Contains(services, s => s is TestRegSingletonAlwaysAdd1);
        Assert.Contains(services, s => s is TestRegSingletonAlwaysAdd2);
    }

    class TestRegSingleton_UserImpl : ITestService1 { }

    [RegisterSingleton(SkipExisting = false)]
    class TestRegSingletonAlwaysAdd1 : ITestService2 { }

    [RegisterSingleton(SkipExisting = false)]
    class TestRegSingletonAlwaysAdd2 : ITestService2 { }

    [Fact]
    public void RegisterSingleton_WithExplicitTypes_RegistersOnlyThoseTypes()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonExplicitTypes1)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonExplicitTypes1>(provider.GetService<ITestService2>());
        Assert.Null(provider.GetService<ITestService1>()); // not in Types
    }

    [Fact]
    public void RegisterSingleton_WithIncompatibleExplicitType_Throws()
    {
        var collection = new ServiceCollection();
        var ex = Assert.Throws<InvalidOperationException>(() =>
            collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonIncompatibleType1))));
        Assert.Contains("ITestService2", ex.Message);
    }

    [RegisterSingleton(typeof(ITestService2))]
    class TestRegSingletonExplicitTypes1 : ITestService1, ITestService2 { }

    [RegisterSingleton(typeof(ITestService2))] // doesn't implement ITestService2
    class TestRegSingletonIncompatibleType1 : ITestService1 { }

    [Fact]
    public void RegisterSingleton_NoSuitableInterfaces_Throws()
    {
        var collection = new ServiceCollection();
        Assert.Throws<InvalidProgramException>(() =>
            collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonNoInterface1))));
    }

    [Fact]
    public void RegisterSingleton_MultipleInterfaces_NameMatchWins()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonMultipleIntfsNameMatch)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonMultipleIntfsNameMatch>(
            provider.GetService<ITestRegSingletonMultipleIntfsNameMatch>());
        Assert.Null(provider.GetService<ITestService1>()); // not the name-matched one
    }

    [Fact]
    public void RegisterSingleton_MultipleInterfaces_NoNameMatch_Throws()
    {
        var collection = new ServiceCollection();
        Assert.Throws<InvalidProgramException>(() =>
            collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonMultipleIntfsNoMatch))));
    }

    [Fact]
    public void RegisterSingleton_ExcludesIDisposable_RegistersRemainingInterface()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonWithDisposable)));
        Assert.IsType<TestRegSingletonWithDisposable>(
            collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void RegisterSingleton_ExcludesIRequestHandlerInterfaces_RegistersRemainingInterface()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonWithRequestHandler)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonWithRequestHandler>(provider.GetService<ITestService1>());
        Assert.Null(provider.GetService<ITestCustomRequestHandler>());
    }

    interface ITestService2 { }
    interface ITestCustomRequestHandler : IRequestHandler { }
    interface ITestRegSingletonMultipleIntfsNameMatch { }

    [RegisterSingleton]
    class TestRegSingletonNoInterface1 { }

    [RegisterSingleton]
    class TestRegSingletonMultipleIntfsNameMatch : ITestRegSingletonMultipleIntfsNameMatch, ITestService1 { }

    [RegisterSingleton]
    class TestRegSingletonMultipleIntfsNoMatch : ITestService1, ITestService2 { }

    [RegisterSingleton]
    class TestRegSingletonWithDisposable : ITestService1, IDisposable
    {
        public void Dispose() { }
    }

    [RegisterSingleton]
    class TestRegSingletonWithRequestHandler : ITestService1, ITestCustomRequestHandler { }

    [Fact]
    public void AddAutoRegisteredServices_SkipsAbstractTypes()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonAbstract)));
        Assert.Null(collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void AddAutoRegisteredServices_SkipsGenericTypes()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonGeneric<>)));
        Assert.Null(collection.BuildServiceProvider().GetService<ITestService2>());
    }

    [Fact]
    public void AddAutoRegisteredServices_SkipsTypesWithoutAttribute()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestNoAttribute)));
        Assert.Null(collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [RegisterSingleton]
    abstract class TestRegSingletonAbstract : ITestService1 { }

    [RegisterSingleton]
    class TestRegSingletonGeneric<T> : ITestService2 { }

    class TestNoAttribute : ITestService1 { }

    [Fact]
    public void AddAutoRegisteredServices_UsesTypeSourceFromCollection()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<ITypeSource>(new MockTypeSource(typeof(TestRegSingleton1)));
        collection.AddAutoRegisteredServices(); // no typeSource argument
        Assert.NotNull(collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void AddAutoRegisteredServices_CollectionNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CoreServiceCollectionExtensions.AddAutoRegisteredServices(null, new MockTypeSource(Array.Empty<Type>())));
    }

    [Fact]
    public void AddAutoRegisteredServices_TypeSourceNullAndNotInCollection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceCollection().AddAutoRegisteredServices(typeSource: null));
    }

    [Fact]
    public void AddAutoRegisteredServices_Predicate_CanFilterOutServices()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingleton1)),
            predicate: (_, _, _) => false);
        Assert.Null(collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void AddAutoRegisteredServices_Predicate_ReceivesCorrectArguments()
    {
        Type capturedServiceType = null, capturedImplType = null;
        RegisterServiceAttribute capturedAttr = null;

        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingleton1)),
            predicate: (svcType, implType, attr) =>
            {
                capturedServiceType = svcType;
                capturedImplType = implType;
                capturedAttr = attr;
                return true;
            });

        Assert.Equal(typeof(ITestService1), capturedServiceType);
        Assert.Equal(typeof(TestRegSingleton1), capturedImplType);
        Assert.IsType<RegisterSingletonAttribute>(capturedAttr);
    }

    [Fact]
    public void RegisterSingleton_WithKey_RegistersAsKeyedService()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonKeyed)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonKeyed>(provider.GetKeyedService<ITestService1>("myKey"));
        Assert.Null(provider.GetService<ITestService1>()); // not registered unkeyed
    }

    [Fact]
    public void RegisterSingleton_WithKey_DoesNotBlockUnkeyedRegistration()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(
            typeof(TestRegSingletonKeyed), typeof(TestRegSingleton1)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonKeyed>(provider.GetKeyedService<ITestService1>("myKey"));
        Assert.IsType<TestRegSingleton1>(provider.GetService<ITestService1>());
    }

    [RegisterSingleton(Key = "myKey")]
    class TestRegSingletonKeyed : ITestService1 { }

    [Fact]
    public void RegisterSingleton_Order_LowerOrderRegisteredFirst_WinsWithSkipExisting()
    {
        var collection = new ServiceCollection();
        // Pass high-order first to ensure Order property drives the result, not scan order
        collection.AddAutoRegisteredServices(new MockTypeSource(
            typeof(TestRegSingletonOrderHigh), typeof(TestRegSingletonOrderLow)));

        Assert.IsType<TestRegSingletonOrderLow>(
            collection.BuildServiceProvider().GetService<ITestService2>());
    }

    [RegisterSingleton(Order = 1)]
    class TestRegSingletonOrderHigh : ITestService2 { }

    [RegisterSingleton(Order = 0)]
    class TestRegSingletonOrderLow : ITestService2 { }

    [Fact]
    public void RegisterSingleton_MultipleAttributes_RegistersAllServiceTypes()
    {
        var collection = new ServiceCollection();
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegSingletonMultipleAttrs)));
        var provider = collection.BuildServiceProvider();

        Assert.IsType<TestRegSingletonMultipleAttrs>(provider.GetService<ITestService1>());
        Assert.IsType<TestRegSingletonMultipleAttrs>(provider.GetService<ITestService2>());
    }

    [RegisterSingleton(typeof(ITestService1)), RegisterSingleton(typeof(ITestService2))]
    class TestRegSingletonMultipleAttrs : ITestService1, ITestService2 { }

    [Fact]
    public void RegisterSingleton_ReplaceExisting_Replaces_Existing_Registration()
    {
        var collection = new ServiceCollection();
        var userImpl = new TestRegSingleton_UserImpl();
        collection.AddSingleton<ITestService1>(userImpl);
        collection.AddAutoRegisteredServices(new MockTypeSource(typeof(TestRegReplaceExisting)));

        Assert.IsType<TestRegReplaceExisting>(collection.BuildServiceProvider().GetService<ITestService1>());
    }

    [Fact]
    public void AddCaching_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CoreServiceCollectionExtensions.AddCaching(null));
    }

    [Fact]
    public void AddCaching_RegistersTwoLevelCache()
    {
        var collection = new ServiceCollection();
        collection.AddCaching();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<ITwoLevelCache>());
    }

    [Fact]
    public void AddTextRegistry_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CoreServiceCollectionExtensions.AddTextRegistry(null));
    }

    [Fact]
    public void AddTextRegistry_RegistersLocalTextRegistryAndLocalizer()
    {
        var collection = new ServiceCollection();
        collection.AddTextRegistry();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<ILocalTextRegistry>());
        Assert.NotNull(provider.GetService<ITextLocalizer>());
    }

    [Fact]
    public void AddAnnotationTypes_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CoreServiceCollectionExtensions.AddAnnotationTypes(null));
    }

    [Fact]
    public void AddAnnotationTypes_RegistersAnnotationTypeRegistry()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<ITypeSource>(new MockTypeSource(Array.Empty<Type>()));
        collection.AddAnnotationTypes();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IAnnotationTypeRegistry>());
    }

    [Fact]
    public void AddTypeSource_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CoreServiceCollectionExtensions.AddTypeSource(null, []));
    }

    [Fact]
    public void AddTypeSource_RegistersTypeSource()
    {
        var collection = new ServiceCollection();
        collection.AddTypeSource([typeof(CoreServiceCollectionExtensionsTests).Assembly]);
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<ITypeSource>());
    }

    [Fact]
    public void AddServiceResolver_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CoreServiceCollectionExtensions.AddServiceResolver(null));
    }

    [Fact]
    public void AddServiceResolver_RegistersServiceResolver()
    {
        var collection = new ServiceCollection();
        collection.AddServiceResolver();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IServiceResolver<ITestService1>>());
    }

    [Fact]
    public void AddFeatureToggles_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CoreServiceCollectionExtensions.AddFeatureToggles(null));
    }

    [Fact]
    public void AddFeatureToggles_RegistersFeatureToggles()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        collection.AddFeatureToggles();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IFeatureToggles>());
    }

    [Fact]
    public void AddUserProvider_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CoreServiceCollectionExtensions.AddUserProvider(null));
    }

    [Fact]
    public void AddUserProvider_RegistersUserProvider()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<IUserAccessor>(new TestUserAccessor());
        collection.AddSingleton<IUserRetrieveService>(new MockUserRetrieveService());
        collection.AddUserProvider();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IUserClaimCreator>());
        Assert.NotNull(provider.GetService<IUserProvider>());
    }

    [Fact]
    public void AddUserProvider_WithAccessorAndRetriever_RegistersServices()
    {
        var collection = new ServiceCollection();
        collection.AddUserProvider<TestUserAccessor, MockUserRetrieveService>();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IUserAccessor>());
        Assert.NotNull(provider.GetService<IUserRetrieveService>());
        Assert.NotNull(provider.GetService<IUserProvider>());
    }

    [Fact]
    public void AddUserProvider_WithAccessorRetrieverAndClaimCreator_RegistersServices()
    {
        var collection = new ServiceCollection();
        collection.AddUserProvider<TestUserAccessor, MockUserRetrieveService, MockUserClaimCreator>();
        var provider = collection.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IUserClaimCreator>());
        Assert.NotNull(provider.GetService<IUserProvider>());
    }

    private class TestUserAccessor : IUserAccessor
    {
        public ClaimsPrincipal User => null;
    }

    private class MockUserClaimCreator : IUserClaimCreator
    {
        public ClaimsPrincipal CreatePrincipal(string username, string authType)
        {
            return new ClaimsPrincipal(new GenericIdentity(username, authType));
        }
    }
}